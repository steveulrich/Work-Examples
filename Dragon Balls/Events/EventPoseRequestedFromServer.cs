
public class EventPoseRequestedFromServer : IEvent
{
    public const string EventName = "EventPoseRequestedFromServer";

    public object GetData()
    {
        return null;
    }

    public string GetName()
    {
        return EventName;
    }
}
