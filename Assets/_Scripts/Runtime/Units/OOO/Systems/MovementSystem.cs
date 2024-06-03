// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Pathfinding;
// using Unity.VisualScripting;
// using UnityEngine;

// public class MovementSystem : BaseUnitSystem
// {
//     AIDestinationSetter _destSetter;
//     FollowerEntity _followerEntity;
//     float _maxSpeed;

//     public MovementSystem(float maxSpeed)
//     {
//         _maxSpeed = maxSpeed;
//     }

//     ~MovementSystem()
//     {
//         _unit.OnStateChanged -= HandleStateChange;
//     }

//     private void HandleStateChange(UnitState state)
//     {
//         switch (state)
//         {
//             case UnitState.IDLE:
//                 break;

//             case UnitState.ATTACKING:
//                 break;

//             default:
//                 break;
//         }
//     }

//     public override void Setup(Unit unit)
//     {
//         base.Setup(unit);

//         _unit.OnStateChanged += HandleStateChange;

//         _destSetter = _unit.UnitGO.GetComponent<AIDestinationSetter>();
//         _followerEntity = _unit.UnitGO.GetComponent<FollowerEntity>();
//         _followerEntity.maxSpeed = _maxSpeed;
//         _followerEntity.stopDistance = HexGrid.Instance.HexSize;
//     }

//     public override void Update()
//     {
//         if (!IsEnabled) return;

//         if (_followerEntity.reachedDestination && _unit.State == UnitState.MOVING)
//         {
//             // _followerEntity.isStopped
//             _destSetter.target = null;
//             _unit.ChangeState(UnitState.ATTACKING);
//         }
//     }

//     public void MoveToTarget(Transform target)
//     {
//         _destSetter.target = target;

//         _unit.ChangeState(UnitState.MOVING);
//     }

//     public Transform GetMoveTarget()
//     {
//         return _destSetter.target;
//     }
// }