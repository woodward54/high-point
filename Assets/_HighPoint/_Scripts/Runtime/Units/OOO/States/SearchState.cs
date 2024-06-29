using System.Linq;
using UnityEngine;

public class SearchState<TStateType> : UnitBaseState<TStateType>
{
    readonly protected float _visionRange;

    public SearchState(Unit unit, float visionRange = float.MaxValue) : base(unit)
    {
        _visionRange = visionRange * visionRange;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // Ensure at least 1 attempt to find a target has been executed
        CalculateNewTarget();

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
        var targetUnit = UnitManager.Instance.Units
            .Where(u => u.Team != OwnUnit.Team)
            .Where(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude < _visionRange)
            .OrderBy(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (targetUnit != null)
            OwnUnit.SetTarget(targetUnit.transform);
        else
            OwnUnit.SetTarget(null);
    }
}