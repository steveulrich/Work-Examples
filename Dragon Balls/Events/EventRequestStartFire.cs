using UnityEngine;
using System.Collections;

public class EventRequestStartFire : IEvent
{
    public const string EventName = "EventRequestStartFire";

    public object GetData()
    {
        return null;
    }

    public string GetName()
    {
        return EventName;
    }
}