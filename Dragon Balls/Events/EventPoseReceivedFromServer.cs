
public class EventPoseReceivedFromServer : IEvent
{
    public const string EventName = "EventPoseReceivedFromServer";

    public object GetData()
    {
        return null;
    }

    public string GetName()
    {
        return EventName;
    }
}
