using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : Singleton<TouchController>
{
    public Action<RaycastHit> OnTouch;

    void Update()
    {
        if (GameManager.Instance.State != GameState.Battle) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                CheckTouch(touch.position);
            }
        }


#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouch(Input.mousePosition);
        }
#endif
    }

    void CheckTouch(Vector2 touchPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        var meshLayer = LayerMask.NameToLayer("Grid");
        int layerMask = 1 << meshLayer;

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            var cell = HexGrid.Instance.GetNearest(hit.point);
            var cellsToCheck = cell.Neighbors;
            cellsToCheck.Add(cell);

            // Don't spawn a unit if near a building
            if (cellsToCheck.Any(c => c.Building != null)) return;

            // float localX = hit.point.x - hit.transform.position.x;
            // float localZ = hit.point.z - hit.transform.position.z;

            float localX = hit.point.x;
            float localZ = hit.point.z;

            SpawnUnit(hit.point);

            OnTouch?.Invoke(hit);
        }
    }

    void SpawnUnit(Vector3 pos)
    {
        var selectedUnit = UnitSelector.Instance.SelectedUnit;
        if (selectedUnit == null) return;

        // Spawn a worker
        var newUnit = UnitManager.Instance.SpawnUnit(selectedUnit, pos, Quaternion.identity);

        // ECS
        // var mvntSystem = new MovementSystem(_selectedAgent.Speed);
        // newUnit.RegisterSystem(mvntSystem);

        // newUnit.RegisterSystem(new WorkerSystem(mvntSystem));
    }
}