public class EventServerAddedClient : IEvent
{
    public const string EventName = "EventServerAddedClient";

    public string clientName;

    public EventServerAddedClient(string name)
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