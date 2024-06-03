using UnityEngine;
using Pathfinding;
using UnityHFSM;
using System;

public class Catapult : AgentUnit
{
    protected override void AddStates()
    {
        FSM.AddState(UnitState.Search, new SearchState<UnitState>(this));
        FSM.AddState(UnitState.Move, new MoveState<UnitState>(this, FollowerEntity, DestSetter));
        FSM.AddState(UnitState.Attack, new AttackState<UnitState>(this, transform, true, 1.0f, true));
        FSM.AddState(UnitState.Dead, new DeadState<UnitState>(this));

        FSM.SetStartState(UnitState.Search);
    }

    protected override void AddTransitions()
    {
        FSM.AddTransition(new Transition<UnitState>(UnitState.Move, UnitState.Search, NeedsNewTarget));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Attack, UnitState.Search, NeedsNewTarget));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Search, UnitState.Move, t => Target != null));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Move, UnitState.Attack, IsTargetInRange));

        FSM.AddTransitionFromAny(UnitState.Dead, t => IsDead);
    }
}