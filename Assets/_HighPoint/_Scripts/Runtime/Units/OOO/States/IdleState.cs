public class IdleState<TStateType> : UnitBaseState<TStateType>
{
    public IdleState(Unit unit) : base(unit)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();

        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.IDLE, 0.2f);
    }
}