using Pathfinding;
using UnityEngine;

public class MoveState<TStateType> : UnitBaseState<TStateType>
{
    FollowerEntity _followerEntity;
    AIDestinationSetter _destSetter;

    public MoveState(Unit unit, FollowerEntity followerEntity, AIDestinationSetter destSetter) : base(unit)
    {
        _followerEntity = followerEntity;
        _destSetter = destSetter;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        _destSetter.target = OwnUnit.Target;

        _followerEntity.canMove = true;

        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.MOVE, 0.2f);
    }

    public override void OnLogic()
    {
        base.OnLogic();
    }

    public override void OnExit()
    {
        base.OnExit();

        _destSetter.target = null;
        _followerEntity.canMove = false;
    }
}