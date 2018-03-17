public class EventCollectiblePickedUp : IEvent
{
    public const string EventName = "EventCollectiblePickedUp";

    private int m_playerID;

    public EventCollectiblePickedUp(int playerID)
    {
        m_playerID = playerID;
    }

    public object GetData()
    {
        return m_playerID;
    }

    public string GetName()
    {
        return EventName;
    }
}
