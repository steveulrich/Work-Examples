using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelocUI : MonoBehaviour, IEventListener
{
    public void SetStatus(bool status)
    {
        gameObject.SetActive(status);
    }

    void Start()
    {
        
        ManageListeners(EventManager.HandleMode.Attach);

        SetStatus(false);
    }

    void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        var evtName = evt.GetName();
        switch (evtName)
        {
            case EventPoseReceivedFromServer.EventName:
                SetStatus(false);
                break;
            case EventPoseRequestedFromServer.EventName:
                SetStatus(true);
                break;
        }

        return ListenerResult.Cascade;
    }

    public void ManageListeners(EventManager.HandleMode mode)
    {
        if (mode == EventManager.HandleMode.Detach && !EventManager.HasInstance())
        {
            return;
        }

        EventManager.ManageListener(mode, this, EventPoseReceivedFromServer.EventName);
        EventManager.ManageListener(mode, this, EventPoseRequestedFromServer.EventName);
    }
}
