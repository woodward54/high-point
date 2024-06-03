using UnityEngine;
using UnityHFSM;

public class Cannon : BuildingUnit
{
    [field: Header("Cannon Properties")]
    [SerializeField] Transform CannonGO;

    protected override void AddStates()
    {
        FSM.AddState(UnitState.Search, new SearchState<UnitState>(this));
        FSM.AddState(UnitState.Attack, new AttackState<UnitState>(this, CannonGO, true, 2f, true));
        FSM.AddState(UnitState.Dead, new DeadState<UnitState>(this));

        FSM.SetStartState(UnitState.Search);
    }

    protected override void AddTransitions()
    {
        FSM.AddTransition(new Transition<UnitState>(UnitState.Attack, UnitState.Search, NeedsNewTarget));
        FSM.AddTransition(new Transition<UnitState>(UnitState.Search, UnitState.Attack, IsTargetInRange));

        FSM.AddTransitionFromAny(UnitState.Dead, t => IsDead);
    }
}