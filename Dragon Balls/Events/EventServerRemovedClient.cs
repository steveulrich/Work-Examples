
public class EventServerRemovedClient : IEvent
{
    public const string EventName = "EventServerRemovedClient";

    public string clientName;

    public EventServerRemovedClient(string name)
    {
        clientName = name;
    }

    public object GetData()
    {
        return clientName;
    }

    public string GetName()
    {
        return EventName;
    }
}