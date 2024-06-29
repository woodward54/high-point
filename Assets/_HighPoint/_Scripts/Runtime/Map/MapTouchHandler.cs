using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapTouchHandler : Singleton<MapTouchHandler>
{
    [SerializeField] LayerMask _raycastLayer;

    ISelectable _selectedObj;

    protected override void OnAwake()
    {
        TouchController.OnTouch += HandleTouch;
    }

    void OnDisable()
    {
        TouchController.OnTouch -= HandleTouch;
    }

    void HandleTouch(Vector2 touchPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _raycastLayer))
        {
            if (hit.collider.TryGetComponent<ISelectable>(out var selectable))
            {
                Select(selectable);
                return;
            }
        }

        // Nothing selected
        NothingSelected();
    }

    void Select(ISelectable selectable)
    {
        // Don't select the same object
        if (selectable == _selectedObj) return;

        // New object selected
        
        _selectedObj?.Unselect();

        selectable.Select();
        _selectedObj = selectable;

        Bus<MapMarkerSelected>.Raise(new MapMarkerSelected(_selectedObj));
    }

    void NothingSelected()
    {
        _selectedObj?.Unselect();
        _selectedObj = null;

        Bus<MapMarkerSelected>.Raise(new MapMarkerSelected(null));
    }
}