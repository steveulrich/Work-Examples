public class EventBallBunchResetRequested : IEvent
{
    public const string EventName = "EventBallBunchResetRequested";

    public string bunch;

    public object GetData()
    {
        return bunch;
    }

    public string GetName()
    {
        return EventName;
    }
}