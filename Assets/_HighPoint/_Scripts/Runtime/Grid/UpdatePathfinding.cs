using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;

public class UpdatePathfinding : Singleton<UpdatePathfinding>
{
    // Unit, IsNodeWalkable
    Queue<(Unit, bool)> _unitUpdatesToProcess = new();

    bool _hasCompleteInitialScan = false;

    EventBinding<GameStateChangedEvent> GameStateChanged;
    EventBinding<UnitSpawnEvent> SpawnedUnitBinding;
    EventBinding<UnitDeathEvent> DeadUnitBinding;

    void OnEnable()
    {
        GameStateChanged = new EventBinding<GameStateChangedEvent>(HandleGameStateChanged);
        Bus<GameStateChangedEvent>.Register(GameStateChanged);

        SpawnedUnitBinding = new EventBinding<UnitSpawnEvent>(HandleUnitSpawn);
        Bus<UnitSpawnEvent>.Register(SpawnedUnitBinding);

        DeadUnitBinding = new EventBinding<UnitDeathEvent>(HandleUnitDied);
        Bus<UnitDeathEvent>.Register(DeadUnitBinding);
    }

    void OnDisable()
    {
        Bus<GameStateChangedEvent>.Unregister(GameStateChanged);
        Bus<UnitSpawnEvent>.Unregister(SpawnedUnitBinding);
        Bus<UnitDeathEvent>.Unregister(DeadUnitBinding);
    }

    void Update()
    {
        if (!_hasCompleteInitialScan) return;

        if (_unitUpdatesToProcess.Count > 0)
        {
            while (_unitUpdatesToProcess.Count > 0)
            {
                var (unit, isWalkable) = _unitUpdatesToProcess.Dequeue();

                AstarPath.active.AddWorkItem(new AstarWorkItem(() =>
                {
                    // Safe to update graphs here
                    var node = AstarPath.active.GetNearest(unit.transform.position).node;

                    if (node != null)
                    {
                        node.Walkable = isWalkable;
                    }
                    else
                    {
                        Debug.LogWarning("No node found for building unit: " + unit.ToString() + ". This should never happen.");
                    }
                }));
            }

            // UpdateGraph();
        }
    }
   
    public void ConnectCells(HexCell cellA, HexCell cellB, PathfindingTag tag)
    {
        // Only 1 ladder/ramp link allowed per tile
        if (cellA.Terrain.GetComponent<NodeLink2>() != null)
            return;

        // TODO: This is slow
        var nl2 = cellA.Terrain.gameObject.AddComponent<NodeLink2>();

        // 1 Ladder covers 2 y spaces, hence div by 2
        nl2.costFactor = Math.Max(1, Math.Abs(cellA.OffsetCoordinates.y - cellB.OffsetCoordinates.y) / 2);
        nl2.end = cellB.Terrain;
        nl2.pathfindingTag = tag;

        Bus<GridUpdatedEvent>.Raise(new GridUpdatedEvent(cellA));
    }

    void HandleGameStateChanged(GameStateChangedEvent @event)
    {
        switch (@event.State)
        {
            case GameState.BuildingBase:
                SetupPathfinding();
                Scan();
                break;
        }
    }

    void HandleUnitSpawn(UnitSpawnEvent @event)
    {
        if (@event.Unit is not BuildingUnit) return;

        _unitUpdatesToProcess.Enqueue((@event.Unit, false));
    }

    void HandleUnitDied(UnitDeathEvent @event)
    {
        if (@event.Unit is not BuildingUnit) return;

        _unitUpdatesToProcess.Enqueue((@event.Unit, true));
    }

    void UpdateGraph()
    {
        var gg = AstarPath.active.data.layerGridGraph;
        gg.RecalculateAllConnections();
    }

    void SetupPathfinding()
    {
        var hexDiameter = HexGrid.Instance.HexSize * 2.0f;
        var nodeSize = hexDiameter * 1.5f / (float)System.Math.Sqrt(2.0f);

        var buffer = 1.75f;

        var widthBuffer = (int)(HexGrid.Instance.Width * buffer);
        var heightBuffer = (int)(HexGrid.Instance.Depth * buffer);

        // These 2 numbers need to be odd for the A* graph to center correctly
        if (widthBuffer % 2 == 0)
            widthBuffer++;

        if (heightBuffer % 2 == 0)
            heightBuffer++;

        // Flipped to better fit the "squished rectangle" from AstarPath
        // AstarPath.active.data.layerGridGraph.SetDimensions(widthBuffer, heightBuffer, nodeSize);
        // AstarPath.active.data.layerGridGraph.center = HexGrid.Instance.ApproxCenterOfGrid();

        AstarPath.active.data.gridGraph.SetDimensions(widthBuffer, heightBuffer, nodeSize);
        AstarPath.active.data.gridGraph.center = HexGrid.Instance.ApproxCenterOfGrid();

        // TODO: HexGrid rotation not yet supported
        // AstarPath.active.data.gridGraph.rotation = HexGrid.Instance.transform.rotation.eulerAngles + new Vector3(0, 90f, 0);
    }

    void Scan()
    {
        // FYI- This is slow
        AstarPath.active.Scan();

        var buildingUnits = UnitManager.Instance.Units.Where(u => u is BuildingUnit);

        var buildingUnitsTuple = buildingUnits.Select(u => (u, false)).ToList();

        buildingUnitsTuple.ForEach(t => _unitUpdatesToProcess.Enqueue(t));

        _hasCompleteInitialScan = true;
    }
}