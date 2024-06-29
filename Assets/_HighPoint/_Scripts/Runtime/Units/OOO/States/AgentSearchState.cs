using System;
using System.Linq;
using Pathfinding;
using UnityEngine;

public class AgentSearchState<TStateType> : SearchState<TStateType>
{
    readonly AgentUnit _agentUnit;
    readonly protected bool _needsPathToTarget;

    public AgentSearchState(AgentUnit unit, bool needsPathToTarget = false, float targetRange = float.MaxValue) : base(unit)
    {
        _agentUnit = unit;
        _needsPathToTarget = needsPathToTarget;
    }

    // public override void OnEnter()
    // {
    //     base.OnEnter();
    // }

    // public override void OnLogic()
    // {
    //     base.OnLogic();
    // }

    protected override void CalculateNewTarget()
    {
        // Find closest dangerous unit or set null
        var possibleTargets = UnitManager.Instance.Units
            .Where(u => u.Team != OwnUnit.Team)
            .Where(u => IsPathPossible(u))
            .Where(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude < _visionRange)
            .OrderBy(u => (u.transform.position - OwnUnit.transform.position).sqrMagnitude);

        Unit targetUnit = possibleTargets.FirstOrDefault();

        // TODO: make a TargetingPriority system, using DamagePerSecond for now
        var possibleDangerousTarget = possibleTargets
            .Where(u => u.DamagePerSecond > 0)
            .FirstOrDefault();

        if (possibleDangerousTarget != null)
        {
            targetUnit = possibleDangerousTarget;
        }

        if (targetUnit != null)
        {
            OwnUnit.SetTarget(targetUnit.transform);
        }
        else
        {
            OwnUnit.SetTarget(null);
        }
    }

    protected virtual bool IsPathPossible(Unit targetUnit)
    {
        if (!_needsPathToTarget) return true;

        var ownUnitNode = AstarPath.active.GetNearest(OwnUnit.transform.position, NNConstraint.Walkable).node;
        var targetNode = AstarPath.active.GetNearest(targetUnit.transform.position, NNConstraint.Walkable).node;

        return PathUtilities.IsPathPossible(ownUnitNode, targetNode);
    }
}