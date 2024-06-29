using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.XR.ARFoundation;
using Drawing;

public class Floor : Singleton<Floor>
{
    public Vector3 FloorPos { get; private set; }

    bool _floorFound = false;

    ARPlaneManager _planeManager;

#if !UNITY_EDITOR
    void OnEnable()
    {
        _planeManager = GetComponent<ARPlaneManager>();

        _planeManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        _planeManager.planesChanged -= OnPlanesChanged;
    }
#endif

    void Start()
    {
#if UNITY_EDITOR
        FoundFloor(Vector3.zero);
#endif
    }

    void Update()
    {
        if (BattleUiController.Instance.DebugMode)
        {
            using (Draw.ingame.WithColor(Color.red))
            {
                Draw.ingame.SolidPlane(FloorPos, Vector3.up, Vector2.one);
            }
        }
    }

    void FoundFloor(Vector3 pos)
    {
        FloorPos = pos;

        HexGrid.Instance.SetGridCenter(FloorPos);
        GameManager.Instance.FoundFloor();

        _floorFound = true;
    }

    public void OnPlanesChanged(ARPlanesChangedEventArgs changes)
    {
        if (_floorFound) return;

        foreach (var plane in changes.added)
        {
            if (plane.classification != UnityEngine.XR.ARSubsystems.PlaneClassification.Floor) continue;

            FoundFloor(plane.center);

            // floorY = Mathf.Min(floorY, plane.center.y);
        }

        // foreach (var plane in changes.updated)
        // {
        //     if (plane.classification != UnityEngine.XR.ARSubsystems.PlaneClassification.Floor) continue;

        //     floorY = Mathf.Min(floorY, plane.center.y);
        // }

        // foreach (var plane in changes.removed)
        // {
        //     if (plane.classification != UnityEngine.XR.ARSubsystems.PlaneClassification.Floor) continue;
        // }
    }
}