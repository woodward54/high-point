using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Unity.Entities.UniversalDelegates;
using Unity.VisualScripting;
using UnityEngine;

public class WorkerSearchState<TStateType> : SearchState<TStateType>
{
    readonly Worker _workerUnit;

    bool _foundValidTarget;

    public WorkerSearchState(Worker unit, float targetRange = float.MaxValue) : base(unit, targetRange)
    {
        _workerUnit = unit;
    }

    protected override void CalculateNewTarget()
    {
        if (WorkerManager.Instance.AllConnectionsMade) return;

        _foundValidTarget = false;

        var unitCell = HexGrid.Instance.GetNearest(OwnUnit.transform.position);

        // Find closest building higher than us
        var PossibleTargetsInOrder = UnitManager.Instance.Units
            .Where(u => u.Team != OwnUnit.Team)
            .Where(u => u is BuildingUnit)
            .Where(u => u.transform.position.y > OwnUnit.transform.position.y)
            .OrderBy(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude)
            .ToList();

        // Check if a path to all targets exists
        foreach (var candidate in PossibleTargetsInOrder)
        {
            var constraint = NNConstraint.Walkable;
            var startNode = AstarPath.active.GetNearest(OwnUnit.transform.position, NNConstraint.Walkable).node;
            var endNode = AstarPath.active.GetNearest(candidate.transform.position, NNConstraint.Walkable).node;

            if (!PathUtilities.IsPathPossible(startNode, endNode))
            {
                // No path exists, lets have the worker build one with ladders & ramps
                SetupPathToBuild(candidate.transform);
                return;
            }
        }

        // All candidate targets have a path, lets start building a shorter path to the next closest building
        foreach (var candidate in PossibleTargetsInOrder)
        {
            if (_foundValidTarget) return;

            SetupPathToBuild(candidate.transform);
        }

        if (_foundValidTarget) return;

        WorkerManager.Instance.AllConnectionsMade = true;
        Debug.LogWarning("Worker: Out of targets, no buildings found, or all connections are already built.");
    }

    void SetupPathToBuild(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning(GetType().Name + ": SetupPathToBuild:  Found null target. This should never happen?...");
            return;
        }

        var targetBuildingCell = HexGrid.Instance.GetNearest(target.position);
        var targetBuildingY = targetBuildingCell.OffsetCoordinates.y;

        // BFS from targetBuildingCell
        //      Find node with the lowest y pos, closest to the worker

        List<HexCell> visitedNodes = new();
        Queue<HexCell> unvisitedNodes = new();
        unvisitedNodes.Enqueue(targetBuildingCell);

        HexCell best2ndChoice = null;
        float closest2ndChoiceTileDist = float.MaxValue;

        while (unvisitedNodes.Count > 0)
        {
            var node = unvisitedNodes.Dequeue();

            // Terrain should never be null because I removed the empty neighbors
            // in FinalizeTerrain(), this is here to appease my sanity
            if (node.Terrain == null)
            {
                Debug.LogWarning(GetType().Name + ": Found null terrain. This should never happen?...");
                continue;
            }

            if (visitedNodes.Contains(node)) continue;

            visitedNodes.Add(node);

            if (node.OffsetCoordinates.y < targetBuildingY &&
                node.Terrain.GetComponent<NodeLink2>() == null)
            {
                // What node are we building the ladder to
                var ladderEndCell = node.Neighbors
                                            .Where(n => n.OffsetCoordinates.y <= targetBuildingY)
                                            .OrderByDescending(n => n.OffsetCoordinates.y).First();

                // End Cell must be higher than start cell
                if (node.OffsetCoordinates.y >= ladderEndCell.OffsetCoordinates.y) continue;

                // Ensure a path exists to the node so the worker can walk there
                var startNode = AstarPath.active.GetNearest(OwnUnit.transform.position).node;
                var endNode = AstarPath.active.GetNearest(node.Terrain.transform.position).node;
                if (PathUtilities.IsPathPossible(startNode, endNode))
                {
                    _workerUnit.LadderStartCell = node;
                    _workerUnit.LadderEndCell = ladderEndCell;

                    OwnUnit.SetTarget(node.Terrain);
                    _foundValidTarget = true;
                    return;
                }
                else
                {
                    // No path exists, lets find the next best choice for recursion
                    var checkDist = Vector3.Distance(node.Terrain.position, OwnUnit.transform.position);
                    if (checkDist < closest2ndChoiceTileDist)
                    {
                        closest2ndChoiceTileDist = checkDist;
                        best2ndChoice = node;
                    }
                }
            }


            foreach (var n in node.Neighbors)
            {
                unvisitedNodes.Enqueue(n);
            }
        }

        // No target found, meaning there is not a way for the worker to get to the target, 
        // try again recursively setting the new target as the best2ndChoice
        SetupPathToBuild(best2ndChoice.Terrain.transform);


        // old code

        // var path = ABPath.Construct(OwnUnit.transform.position, target.transform.position, onPathComplete);

        // void onPathComplete(Path p)
        // {
        //     // Find closest cell to the targetBuilding that we can walk to
        //     var targetReachableNode = (Vector3)p.path.Last().position;

        //     // The last cell the unit is able to walk to
        //     var moveToCell = HexGrid.Instance.GetNearest(targetReachableNode);

        //     // This cell already has a completed vertical connection
        //     if (moveToCell.Terrain.GetComponent<NodeLink2>() != null) return;

        //     // What node are we building the ladder to
        //     var ladderEndCell = moveToCell.Neighbors
        //                                         .Where(n => n.OffsetCoordinates.y <= buildingY)
        //                                         .OrderByDescending(n => n.OffsetCoordinates.y).First();

        //     // End Cell must be higher than start cell
        //     if (moveToCell.OffsetCoordinates.y >= ladderEndCell.OffsetCoordinates.y) return;

        //     _workerUnit.LadderStartCell = moveToCell;
        //     _workerUnit.LadderEndCell = ladderEndCell;

        //     OwnUnit.SetTarget(moveToCell.Terrain);

        //     _foundValidTarget = true;
        // }

        // AstarPath.StartPath(path);
    }
}