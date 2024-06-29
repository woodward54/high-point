using System;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.Profiling;

public class AgentConfusedState<TStateType> : MoveState<TStateType>
{
    readonly AgentUnit _agentUnit;

    bool _reachedTarget;

    public AgentConfusedState(AgentUnit unit, FollowerEntity followerEntity, AIDestinationSetter destSetter) : base(unit, followerEntity, destSetter, needsExitTime: true)
    {
        _agentUnit = unit;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        var target = GetRandWalkPoint();
        OwnUnit.SetTarget(target);
        _destSetter.target = target;

        _reachedTarget = false;
    }

    public override void OnLogic()
    {
        base.OnLogic();

        if (!_reachedTarget && _agentUnit.IsTargetInRange(null))
        {
            OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.IDLE, 0.2f);
            OwnUnit.DisplayPopupMessage("???");
            _reachedTarget = true;
        }

        // When the state has been active for more than 5 seconds,
        // notify the fsm that the state can cleanly exit.
        if (_reachedTarget && timer.Elapsed > 5)
            fsm.StateCanExit();
    }

    public override void OnExit()
    {
        base.OnExit();

        OwnUnit.SetTarget(null);
    }

    Transform GetRandWalkPoint()
    {
        var ownUnitCell = HexGrid.Instance.GetNearest(OwnUnit.transform.position);
        var reachableNodes = new List<HexCell>() { ownUnitCell };
        reachableNodes.AddRange(ownUnitCell.Neighbors);
        reachableNodes.RemoveAll(n => !HexGrid.Instance.IsPathPossible(ownUnitCell, n));

        var randIndex = UnityEngine.Random.Range(0, reachableNodes.Count);

        return reachableNodes[randIndex].Terrain;
    }
}