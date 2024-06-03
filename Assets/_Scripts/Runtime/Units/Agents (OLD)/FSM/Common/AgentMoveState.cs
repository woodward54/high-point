// using UnityEngine;

// public class AgentMoveState : UnitStateBase<AgentStates>
// {
//     public AgentMoveState(UnitBase<AgentStates> unit) : base(unit)
//     {
//         OwnAgent = unit as AgentBase;
//     }

//     private Vector3 LastDestination;
//     private AgentBase OwnAgent;

//     public override void OnEnter()
//     {
//         base.OnEnter();

//         // TODO
//         // NavMeshAgent.isStopped = false;

//         Animator.CrossFade(AnimatorStates.MOVE, 0.15f);
//         // Just take the point if we click on the floor or if there's no collider
//         // Otherwise, don't walk inside that object!
//         LastDestination = GetMoveTargetLocation();

//         // TODO
//         // NavMeshAgent.SetDestination(LastDestination);
//     }

//     private Vector3 GetMoveTargetLocation()
//     {
//         Vector3 target;
//         if (Unit.TransformTarget == null)
//         {
//             target = Unit.Target;
//         }
//         else
//         {
//             target = Unit.TransformTarget.position;
//         }

//         return target;
//     }

//     public override void OnLogic()
//     {
//         base.OnLogic();
//         Vector3 target = GetMoveTargetLocation();

//         if (target != LastDestination)
//         {
//             LastDestination = target;

//             // TODO
//             // NavMeshAgent.SetDestination(target);
//         }

//         // if (Enemy.NearbyLlamas.Count == 0)
//         // {
//         //     return;
//         // }

//         // Vector3 directionToClosestLlama = (Enemy.NearbyLlamas[0].transform.position - Enemy.transform.position);

//         // Enemy.Agent.velocity = Vector3.Lerp(
//         //     Enemy.Agent.desiredVelocity,
//         //     -directionToClosestLlama.normalized * Enemy.Agent.speed * Enemy.Unit.RunAwayConfig.SpeedMultiplier,
//         //     Mathf.Clamp01((Enemy.Unit.RunAwayConfig.RunCompletelyAwayDistance.x - directionToClosestLlama.magnitude) / Enemy.Unit.RunAwayConfig.RunCompletelyAwayDistance.y)
//         // );
//     }
// }