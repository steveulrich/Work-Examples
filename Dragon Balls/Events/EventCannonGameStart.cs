using UnityEngine;
using System.Collections;

public class EventCannonGameStart : IEvent
{
    public const string EventName = "EventCannonGameStart";

    public int dummy;

    public object GetData()
    {
        return dummy;
    }

    public string GetName()
    {
        return EventName;
    }
}