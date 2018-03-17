using UnityEngine;
using System.Collections;

public class EventStartFire : IEvent
{
    public const string EventName = "EventStartFire";

    public object GetData()
    {
        return null;
    }

    public string GetName()
    {
        return EventName;
    }
}
