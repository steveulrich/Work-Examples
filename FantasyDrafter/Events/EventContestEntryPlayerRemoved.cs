using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventContestEntryPlayerRemoved : IEvent
{
    public const string EventName = "EventContestEntryPlayerRemoved";

    FantasyPlayer removedPlayerId;

    public EventContestEntryPlayerRemoved(FantasyPlayer player)
    {
        removedPlayerId = player;
    }

    public object GetData()
    {
        return removedPlayerId;
    }

    public string GetName()
    {
        return EventName;
    }
}