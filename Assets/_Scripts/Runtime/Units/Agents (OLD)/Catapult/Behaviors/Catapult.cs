// using UnityEngine;
// using UnityHFSM;

// public class Catapult : AgentBase
// {
//     protected override void AddStates()
//     {
//         FSM.AddState(AgentStates.Idle, new IdleState<AgentStates>(this));
//         FSM.AddState(AgentStates.Move, new AgentMoveState(this));
//         FSM.SetStartState(AgentStates.Idle);
//     }

//     protected override void AddTransitions()
//     {
//         FSM.AddTriggerTransitionFromAny(StateEvent.MoveIssued, AgentStates.Move);
//         FSM.AddTriggerTransitionFromAny(StateEvent.StopIssued, AgentStates.Idle);

//         FSM.AddTransition(new Transition<AgentStates>(AgentStates.Move, AgentStates.Move, ShouldPickNewWanderLocation));

//         FSM.AddTriggerTransitionFromAny(StateEvent.Die, AgentStates.Idle, null, (_) =>
//         {
//             gameObject.SetActive(false);
//         });
//     }
   
//     private void Disable()
//     {
//         FSM.Trigger(StateEvent.Die);
//     }

//     // TODO - is this right?
//     private bool IsCloseToTarget(Transition<AgentStates> _) =>
//         Pathfinder.enabled && Pathfinder.remainingDistance <= Pathfinder.stopDistance;

//     private bool ShouldPickNewWanderLocation(Transition<AgentStates> _) => IsWandering && IsCloseToTarget(_);
// }