public class DeadState<TStateType> : UnitBaseState<TStateType>
{
    public DeadState(Unit unit) : base(unit)
    {

    }

    public override void OnEnter()
    {
        base.OnEnter();

        OwnUnit.Animator.CrossFadeInFixedTime(AnimatorStates.DEAD, 0.2f);
    }
}