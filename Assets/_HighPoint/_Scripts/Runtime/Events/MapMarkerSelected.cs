public struct MapMarkerSelected : IEvent
{
    public ISelectable Selected;

    public MapMarkerSelected(ISelectable selected)
    {
        Selected = selected;
    }
}