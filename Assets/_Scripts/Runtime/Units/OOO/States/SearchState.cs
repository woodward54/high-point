using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SearchState<TStateType> : UnitBaseState<TStateType>
{
    readonly protected float _targetRangeSquared;

    public SearchState(Unit unit, float targetRange = float.MaxValue) : base(unit)
    {
        _targetRangeSquared = targetRange * targetRange;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.IDLE, 0.2f);
    }

    public override void OnLogic()
    {
        base.OnLogic();

        CalculateNewTarget();
    }

    protected virtual void CalculateNewTarget()
    {
        // Find closest unit or set null
        var TargetUnit = UnitManager.Instance.Units
            .Where(u => !u.IsDead && u.Team != OwnUnit.Team)
            .Where(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude < _targetRangeSquared)
            .OrderBy(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (TargetUnit != null)
            OwnUnit.SetTarget(TargetUnit.transform);
        else
            OwnUnit.SetTarget(null);
    }
}