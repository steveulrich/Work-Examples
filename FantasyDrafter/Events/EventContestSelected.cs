
public class EventContestSelected : IEvent
{
    public const string EventName = "EventContestSelected";

    ContestInfo contest;

    public EventContestSelected(ContestInfo selectedContest)
    {
        contest = selectedContest;
    }

    public object GetData()
    {
        return contest;
    }

    public string GetName()
    {
        return EventName;
    }
}
