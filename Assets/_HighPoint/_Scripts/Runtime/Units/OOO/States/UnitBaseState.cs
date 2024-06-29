using UnityHFSM;

public abstract class UnitBaseState<TStateType> : State<TStateType>
{
    protected Unit OwnUnit;

    public UnitBaseState(Unit unit, bool needsExitTime = false) : base(needsExitTime: needsExitTime)
    {
        OwnUnit = unit;
    }
}