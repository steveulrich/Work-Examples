public class EventGameReadyToStart : IEvent
{
    public const string EventName = "EventGameReadyToStart";

    public object GetData()
    {
        return null;
    }

    public string GetName()
    {
        return EventName;
    }
}
