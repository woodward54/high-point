public struct GameStateChangedEvent : IEvent
{
    public GameState State;

    public GameStateChangedEvent(GameState newState)
    {
       State = newState; 
    }
}