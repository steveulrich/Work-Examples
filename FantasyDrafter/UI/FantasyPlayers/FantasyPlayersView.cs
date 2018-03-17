using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FantasyPlayersView : MainMenuViewBase, IEventListener
{
    public FantasyPlayersSlot FantasySlotPrefab;

    public Transform SlotsContentParent;

    public List<FantasyPlayersSlot> FantasyPlayerSlots;

	// Use this for initialization
	void Start ()
    {
        ManageListeners(EventManager.HandleMode.Attach);
	}

    private void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    public void SetupFantasySlots()
    {
        foreach (var player in FantasyManager.Instance.FantasyPlayersList)
        {
            FantasyPlayersSlot playerSlot = Instantiate(FantasySlotPrefab) as FantasyPlayersSlot;
            if (playerSlot != null)
            {
                playerSlot.Configure(player.Name, player.Salary, player.Team, player.Points);
                playerSlot.transform.SetParent(SlotsContentParent);
                playerSlot.transform.localScale = Vector3.one;
                playerSlot.transform.localPosition = Vector3.zero;
                playerSlot.transform.localRotation = Quaternion.identity;
            }
        }
    }

    public override void Activate(object[] optionalParams = null)
    {
        base.Activate();

        // Iterate over the FantasyPlayerSlots list and disable those who have already been claimed by the player for their roster
        foreach(var slot in FantasyPlayerSlots)
        {
            //if(User.ActiveUser.CurrentWeekRoster.Contains(slot.NameText.Name)
            {
                //slot.gameObject.SetActive(false);
            }
        }
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        var evtName = evt.GetName();
        switch(evtName)
        {
            case EventFantasyPlayersListUpdated.EventName:
                SetupFantasySlots();
                break;
        }

        return ListenerResult.Cascade;
    }

    public void ManageListeners(EventManager.HandleMode mode)
    {
        if(mode == EventManager.HandleMode.Detach && !EventManager.HasInstance())
        {
            return;
        }

        EventManager.ManageListener(mode, this, EventFantasyPlayersListUpdated.EventName);
    }
}
