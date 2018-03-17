
public class EventBallBunchResetPressed : IEvent
{
    public const string EventName = "EventBallBunchResetPressed";

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