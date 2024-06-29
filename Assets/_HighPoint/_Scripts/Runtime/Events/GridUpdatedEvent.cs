public struct GridUpdatedEvent : IEvent
{
    public HexCell UpdatedCell;

    public GridUpdatedEvent(HexCell updatedCell)
    {
        UpdatedCell = updatedCell;
    }
}