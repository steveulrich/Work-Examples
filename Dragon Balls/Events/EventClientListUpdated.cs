public class EventClientListUpdated : IEvent
{
    public const string EventName = "EventClientListUpdated";

    public string clientList;

    public EventClientListUpdated(string newList)
    {
        clientList = newList;
    }

    public object GetData()
    {
        return clientList;
    }

    public string GetName()
    {
        return EventName;
    }
}
