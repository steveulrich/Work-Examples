using UnityEngine;
using System.Collections;

public class EventCannonGameRequested : IEvent
{
    public const string EventName = "EventCannonGameRequested";

    public object GetData()
    {
        return null;
    }

    public string GetName()
    {
        return EventName;
    }
}