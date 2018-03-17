
public class EventClientAvatarSelected : IEvent
{
    public const string EventName = "EventClientAvatarSelected";

    public int EggIndex;

    public EventClientAvatarSelected(int index)
    {
        EggIndex = index;
    }

    public object GetData()
    {
        return EggIndex;
    }

    public string GetName()
    {
        return EventName;
    }
}
