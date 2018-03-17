
public class EventRequestBallBunchRelease : IEvent
{
    public const string EventName = "EventRequestBallBunchRelease";

    public int bunchIndex;

    public EventRequestBallBunchRelease(int ballBunch)
    {
        bunchIndex = ballBunch;
    }

    public object GetData()
    {
        return bunchIndex;
    }

    public string GetName()
    {
        return EventName;
    }
}