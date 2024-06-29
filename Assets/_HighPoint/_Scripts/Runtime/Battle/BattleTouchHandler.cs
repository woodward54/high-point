using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTouchHandler : Singleton<BattleTouchHandler>
{
    [SerializeField] LayerMask _raycastLayer;

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
        if (GameManager.Instance.State != GameState.Battle) return;

        var selectedUnit = UnitSelector.Instance.SelectedUnit;
        if (selectedUnit == null) return;

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _raycastLayer))
        {
            var cell = HexGrid.Instance.GetNearest(hit.collider.bounds.center);

            // Only allowed to spawn units at y==0
            if (cell.OffsetCoordinates.y > 0)
            {
                var nonPlaceableArea = HexGrid.Instance.GetAllCells()
                                            .Where(c => c.OffsetCoordinates.y > 0)
                                            .ToList();

                nonPlaceableArea.ForEach(c => c.Terrain.GetComponent<MeshRenderer>()
                                    .FlashColor(Color.red, 1f, false));

                return;
            }


            var cellsToCheck = cell.Neighbors;
            cellsToCheck.Add(cell);

            // Don't spawn a unit if near a building, flash invalid cells red
            if (cellsToCheck.Any(c => c.Building != null))
            {
                var nonPlaceableArea = UnitManager.Instance.Units
                    .Where(u => u is BuildingUnit)
                    .Select(b => HexGrid.Instance.GetNearest(b.transform.position))
                    .SelectMany(c => new List<HexCell> { c }.Concat(c.Neighbors))
                    .ToHashSet()
                    .ToList();

                nonPlaceableArea.ForEach(c => c.Terrain.GetComponent<MeshRenderer>()
                                                                .FlashColor(Color.red, 1f, false));

                return;
            }

            // float localX = hit.point.x - hit.transform.position.x;
            // float localZ = hit.point.z - hit.transform.position.z;

            float localX = hit.point.x;
            float localZ = hit.point.z;

            var newUnit = UnitInventory.Instance.SpawnUnit(selectedUnit, hit.point, Quaternion.identity);
        }
    }

}