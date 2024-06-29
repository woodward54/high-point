public struct UnitSpawnEvent : IEvent
{
    public Unit Unit;

    public UnitSpawnEvent(Unit unit)
    {
        Unit = unit;
    }
}