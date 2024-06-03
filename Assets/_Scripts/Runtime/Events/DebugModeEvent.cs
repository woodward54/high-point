public struct DebugModeEvent : IEvent
{
    public bool DebugMode;

    public DebugModeEvent(bool newState)
    {
        DebugMode = newState;
    }
}