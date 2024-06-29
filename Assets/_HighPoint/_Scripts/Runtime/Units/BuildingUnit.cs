using UnityEngine;
using UnityHFSM;

public class BuildingUnit : Unit
{
    protected override void AddStates()
    {
        FSM.AddState(UnitState.Idle, new IdleState<UnitState>(this));
        FSM.AddState(UnitState.Dead, new DeadState<UnitState>(this));

        FSM.SetStartState(UnitState.Idle);
    }

    protected override void AddTransitions()
    {
        FSM.AddTransitionFromAny(UnitState.Dead, t => IsDead);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Die()
    {
        var cell = HexGrid.Instance.GetNearest(transform.position);
        cell.Building = null;
        base.Die();
    }

    protected bool IsTargetInRange(Transition<UnitState> _) => Target != null &&
           (Target.transform.position - transform.position).sqrMagnitude <= AttackRange * AttackRange;

    protected bool NeedsNewTarget(Transition<UnitState> _)
    {
        if (Target == null) return true;

        if (!IsTargetInRange(_)) return true;

        var unit = Target.GetComponent<Unit>();
        if (unit != null && unit.IsDead) return true;

        return false;
    }
}