
public class EventFantasyPlayersListUpdated : IEvent
{
    public const string EventName = "EventFantasyPlayersListUpdated";

    System.DateTime DateUpdated;

    public EventFantasyPlayersListUpdated(System.DateTime dateUpdated)
    {
        DateUpdated = dateUpdated;
    }

    public object GetData()
    {
        return DateUpdated;
    }

    public string GetName()
    {
        return EventName;
    }
}
