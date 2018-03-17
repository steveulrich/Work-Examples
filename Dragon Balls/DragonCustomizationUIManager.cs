using UnityEngine;
using System.Collections;

public class DragonCustomizationUIManager : MonoBehaviour, IEventListener
{
    public DragonEgg[] DragonOptions;

    private bool dragonSelected = false;

    void Start()
    {
        for(int i = 0; i < DragonOptions.Length; i++)
        {
            DragonOptions[i].EggIndex = i;
        }

        ManageListeners(EventManager.HandleMode.Attach);
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
            case EventClientAvatarSelected.EventName:
                if(!dragonSelected)
                {
                    int eggIndex = (int)evt.GetData();
                    OnClickDragonEgg(eggIndex);
                    dragonSelected = true;
                }
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

        EventManager.ManageListener(mode, this, EventClientAvatarSelected.EventName);
    }

    public void OnClickDragonEgg(int eggIndex)
    {
        int egg = eggIndex;
        System.Action initializeClientCB = delegate
        {
            EventManager.instance.TriggerEvent(new EventClientAvatarSelected(egg));

            DragonNetworkManager.singleton.playerPrefab = DragonNetworkManager.singleton.spawnPrefabs[egg];
            Debug.Log("initializeClientCB |||| Spawn a --- " + DragonNetworkManager.singleton.playerPrefab.name);

            DragonNetworkManager.singleton.StartClient();
        };

        DragonOptions[eggIndex].CrackEgg(initializeClientCB);

    }

}
