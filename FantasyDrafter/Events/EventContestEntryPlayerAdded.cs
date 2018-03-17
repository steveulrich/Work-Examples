
public class EventContestEntryPlayerAdded : IEvent
{
    public const string EventName = "EventContestEntryPlayerAdded";

    FantasyPlayer addedPlayerId;

    public EventContestEntryPlayerAdded(FantasyPlayer player)
    {
        addedPlayerId = player;
    }

    public object GetData()
    {
        return addedPlayerId;
    }

    public string GetName()
    {
        return EventName;
    }
}