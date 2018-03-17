using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameUIManager : NetworkBehaviour, IEventListener
{

    public void OnClickPlay(int playerID)
    {
        Debug.Log(playerID + " player clicked Ready for Coin Game");
        
        // @TODO - Move Dragon to starting position

        if(connectionToServer != null)
        {
            // Send server Player Ready Command
            CmdReadyPlayer(playerID);
        }
        else
        {
            Debug.Log("not connected to server");
        }
        
    }

    [Command]
    private void CmdReadyPlayer(int playerID)
    {
        Debug.Log("Server received player ***" + playerID + "*** ready for Coin Game");

        // Server will ready that player and check internally if the game is ready to begin.
        // If the game is ready to begin, we will receive a "GameIsReadyToStart" event
        ServerManager.Instance.ReadyPlayer(playerID, true);
    }

    public void SwitchOffUI()
    {
        GetComponent<Canvas>().enabled = false;
    }

    #region EventListener

    private void Start()
    {
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
            case EventGameReadyToStart.EventName:
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

        EventManager.ManageListener(mode, this, EventGameReadyToStart.EventName);
    }
    #endregion

}
