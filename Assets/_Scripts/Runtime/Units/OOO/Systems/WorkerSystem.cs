// using System.Collections.Generic;
// using System.Linq;
// using Pathfinding;
// using Drawing;
// using TMPro;
// using Unity.VisualScripting;
// using Unity.VisualScripting.Dependencies.NCalc;
// using UnityEngine;

// public enum WorkerState
// {
//     IDLE,
//     MOVING,
//     BUILDING
// }

// public abstract class BaseAgentLogicSystem : BaseUnitSystem {
//     // public WorkerState State { get; private set; }
// }

// public class WorkerSystem : BaseUnitSystem
// {
//     MovementSystem _movement;

//     float _ladderBuildTime = 5f;
//     float _lastBuildTime = 0;

//     HexCell _ladderStartCell;
//     HexCell _ladderEndCell;
//     int _ladderBuiltIndex;
//     int _laddersToBuild;

//     public WorkerSystem(MovementSystem movementSystem)
//     {
//         _movement = movementSystem;
//     }

//     ~WorkerSystem()
//     {
//         // _unit.OnStateChanged -= HandleStateChange;
//     }

//     public override void Setup(Unit unit)
//     {
//         base.Setup(unit);

//         // _unit.OnStateChanged += HandleStateChange;

//         // tmp 
//         CalculateNewTarget();
//     }

//     public override void Update()
//     {
//         if (!IsEnabled) return;

//         // if (_unit.State == UnitState.ATTACKING)
//         // {
//         //     if (_lastBuildTime + _ladderBuildTime > Time.time)
//         //     {
//         //         _lastBuildTime = Time.time;
//         //         BuildLadder();
//         //     }
//         // }
//     }

//     // private void HandleStateChange(UnitState state)
//     // {
//     //     switch (state)
//     //     {
//     //         case UnitState.IDLE:
//     //             CalculateNewTarget();
//     //             break;

//     //         case UnitState.ATTACKING:
//     //             _lastBuildTime = Time.time;
//     //             break;

//     //         default:
//     //             break;
//     //     }
//     // }

//     void CalculateNewTarget()
//     {
//         var unitPos = _unit.UnitGO.transform.position;
//         // var unitCell = HexGrid.Instance.GetNearest(unitPos);

//         // Get all enemy building units
//         var buildingUnits = UnitManager.Instance.Units.Where(u =>
//                                                     (u.Team != _unit.Team) &
//                                                     (u.Type == UnitType.BUILDING)).ToList();

//         // Find closest building above y=0
//         HexCell closestBuildingCell = null;
//         float closestDist = float.MaxValue;
//         foreach (var building in buildingUnits)
//         {
//             var pos = building.UnitGO.transform.position;

//             var cell = HexGrid.Instance.GetNearest(pos);

//             if (cell.OffsetCoordinates.y > 0)
//             {
//                 var dist = Vector3.Distance(unitPos, pos);
//                 if (dist < closestDist)
//                 {
//                     closestBuildingCell = cell;
//                     closestDist = dist;
//                 }
//             }
//         }

//         if (closestBuildingCell == null)
//         {
//             Debug.LogWarning("Worker couldn't find closest building!");
//             return;
//         }

//         // Find closest tile to the targetBuilding on our Y level (that we can walk to)

//         var startNode = AstarPath.active.GetNearest(_unit.UnitGO.transform.position).node;
//         var endNode = AstarPath.active.GetNearest(closestBuildingCell.Terrain.transform.position).node;

//         if (PathUtilities.IsPathPossible(startNode, endNode))
//         {
//             // TODO: There already exists a path to the closestBuilding
//             // What should the worker do here? should we find a new path to build?
//             return;
//         }

//         // No path exists, lets have the worker build one with ladders and ramps
//         var path = ABPath.Construct(unitPos, closestBuildingCell.Terrain.transform.position, onPathComplete);

//         void onPathComplete(Path p)
//         {
//             var targetReachableNode = (Vector3)p.path.Last().position;

//             var moveToCell = HexGrid.Instance.GetNearest(targetReachableNode);

//             // What node are we building the ladder to
//             var buildingY = closestBuildingCell.OffsetCoordinates.y;
//             // var tiles = HexGrid.Instance.GetCellRegionsByHeight().Where(g => g.Key == buildingY);

//             // TODO ladder vs ramp logic


//             _ladderStartCell = moveToCell;
//             _ladderEndCell = moveToCell.Neighbors.Where(n => n.OffsetCoordinates.y <= closestBuildingCell.OffsetCoordinates.y)
//                                                 .OrderByDescending(n => n.OffsetCoordinates.y).First();

//             _ladderBuiltIndex = _ladderStartCell.OffsetCoordinates.y;
//             _laddersToBuild = _ladderEndCell.OffsetCoordinates.y - _ladderStartCell.OffsetCoordinates.y;

//             // TODO
//             // _ladderHexSide = HexUtils.NeighborConnectionSideIndex(_ladderStartCell, _ladderEndCell);

//             using (Draw.WithDuration(30f))
//             {
//                 using (Draw.WithLineWidth(2.5f))
//                 {
//                     Draw.Cross(_ladderStartCell.Terrain.position, 0.1f, Color.red);
//                     Draw.Cross(_ladderEndCell.Terrain.position, 0.1f, Color.blue);
//                 }
//             }

//             _movement.MoveToTarget(moveToCell.Terrain);
//         }


//         AstarPath.StartPath(path);
//     }



//     void BuildLadder()
//     {
//         if (_ladderBuiltIndex >= _laddersToBuild) return;


//         // TODO we need this function

//         // Convert _ladderBuiltIndex (y int coordinate) to world position
//         // var yPosWorld = HexUtils.();
//         // SpawnLadder(_ladderStartCell, _ladderHexSide, yPosWorld);


//     }

//     void FinalizeConnection()
//     {
//         // Finish building the latter
//         // Connect two nodes
//         // var node1 = AstarPath.active.GetNearest(transform.position, NNConstraint.None).node;
//         // var node2 = AstarPath.active.GetNearest(transform.position + Vector3.right, NNConstraint.None).node;
//         // var cost = (uint)(node2.position - node1.position).costMagnitude;

//         // GraphNode.Connect(node1, node2, cost, OffMeshLinks.Directionality.TwoWay);
//     }

//     // 0 is bottom right side
//     void SpawnLadder(HexCell cell, int sideIndex, float yPos)
//     {
//         var center = HexGrid.Instance.GetHexPosition( cell.OffsetCoordinates, HexGrid.Instance.Orientation);
//         var corners = HexUtils.Corners(HexGrid.Instance.HexSize, HexGrid.Instance.Orientation);

//         var a = center + corners[sideIndex % 6];
//         var b = center + corners[(sideIndex + 1) % 6];

//         var pos = (a + b) / 2f;
//         pos.y = yPos;

//         var rot = Vector3.zero;
//         rot.y = 180f + HexUtils.SideAngle(sideIndex);

//         var ladderGO = Object.Instantiate(
//             BaseGenerator.Instance.Ladder,
//             pos,
//             Quaternion.Euler(rot));

//         ladderGO.transform.localScale *= HexGrid.Instance.HexSize;
//     }
// }