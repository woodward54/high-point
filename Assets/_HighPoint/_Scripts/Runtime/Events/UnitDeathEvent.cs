public struct UnitDeathEvent : IEvent
{
    public Unit Unit;

    public UnitDeathEvent(Unit unit)
    {
        Unit = unit;
    }
}