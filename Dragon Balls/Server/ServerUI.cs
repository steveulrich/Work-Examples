using UnityEngine;
using UnityEngine.UI;

public class ServerUI : MonoBehaviour, IEventListener
{
    public ServerManager ServerManager;
    public Text ServerIPAddress;
    public Text ServerPort;
    public Text ClientCount;
    public Text ClientList;

    // Use this for initialization
    void Start()
    {
        ServerIPAddress.text = DragonNetworkManager.singleton.networkAddress;
        ServerPort.text = DragonNetworkManager.singleton.networkPort.ToString();

        ManageListeners(EventManager.HandleMode.Attach);
    }

    // Update is called once per frame
    void Update()
    {
        // @TODO: Replace this with an event for new player connected/disconnected to update the ClientCount text
        ClientCount.text = DragonNetworkManager.singleton.numPlayers.ToString();
    }

    public void OnPressDisconnect()
    {
        DragonNetworkManager.singleton.StopServer();
    }


    #region EventListener

    void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        var evtName = evt.GetName();
        switch (evtName)
        {
            case EventClientListUpdated.EventName:
                string newClientList = (string)evt.GetData();
                ClientList.text = newClientList;
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

        EventManager.ManageListener(mode, this, EventClientListUpdated.EventName);
    }
    #endregion
}
