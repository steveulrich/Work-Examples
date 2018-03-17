using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ServerManager : NetworkBehaviour, IEventListener
{
    public static ServerManager Instance = null;

    public System.Text.StringBuilder ClientList = new System.Text.StringBuilder();

    private string _lineBreak = "\n";

    public bool[] ReadyStatusCoinGame = new bool[2];

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);

        SetupServerCamera();

        ManageListeners(EventManager.HandleMode.Attach);
    }

    private void SetupServerCamera()
    {
        GameObject.Find("SvrCamera").SetActive(false);

        Camera cam = new GameObject().AddComponent<Camera>();
        cam.gameObject.AddComponent<AudioListener>();
        cam.tag = "MainCamera";

        cam.transform.SetParent(transform);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;
    }

    private void AddNameToClientList(string name)
    {
        ClientList.Append(_lineBreak).Append(name);

        EventManager.instance.TriggerEvent(new EventClientListUpdated(ClientList.ToString()));
    }

    private void RemoveNameFromClientList(string name)
    {
        ClientList.Replace(_lineBreak + name, string.Empty);

        EventManager.instance.TriggerEvent(new EventClientListUpdated(ClientList.ToString()));
    }

    [ClientRpc]
    public void RpcCollectiblePickedUp(int playerID)
    {
        EventManager.instance.TriggerEvent(new EventCollectiblePickedUp(playerID));
    }

    [ClientRpc]
    public void RpcDisableGameCollectible(GameObject collectible)
    {
        collectible.SetActive(false);
    }

    #region Server Game Code
    public void ReadyPlayer(int playerID, bool readyStatus)
    {
        ReadyStatusCoinGame[playerID] = readyStatus;

        if(CheckReadyStatus())
        {
            Debug.Log("Send RPC to start the game");
            RpcStartGame();
        }
    }

    [ClientRpc]
    public void RpcStartGame()
    {
        EventManager.instance.TriggerEvent(new EventGameReadyToStart());
    }

    private bool CheckReadyStatus()
    {
        for(int i = 0; i < ReadyStatusCoinGame.Length; i++)
        {
            if(ReadyStatusCoinGame[i] == false)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

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
            case EventServerAddedClient.EventName:
                string addedClientName = (string)evt.GetData();
                AddNameToClientList(addedClientName);
                break;
            case EventServerRemovedClient.EventName:
                string removedClientName = (string)evt.GetData();
                RemoveNameFromClientList(removedClientName);
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

        EventManager.ManageListener(mode, this, EventServerAddedClient.EventName);
        EventManager.ManageListener(mode, this, EventServerRemovedClient.EventName);
    }
    #endregion
}
