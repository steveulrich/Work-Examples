using UnityEngine;
using System.Collections;

public class EventBallBunchPressed : IEvent
{
    public const string EventName = "EventBallBunchReleaseRequested";

    public BallReleaseHandler bunch;

    public EventBallBunchPressed(BallReleaseHandler ballBunch)
    {
        bunch = ballBunch;
    }

    public object GetData()
    {
        return bunch;
    }

    public string GetName()
    {
        return EventName;
    }
}
