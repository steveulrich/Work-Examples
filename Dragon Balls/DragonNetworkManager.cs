using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;


public class DragonNetworkManager : NetworkManager, IEventListener
{
    private int avatarIndex = -1;

    private int playerIDCount = 0;

    public static string RelocURL;

    #region NetworkManager Overrides
    public override void OnStartServer()
    {
        base.OnStartServer();

        Debug.Log("Server Started! -- " + networkAddress + " :: " + networkPort);

    }

    public override void OnStartClient(NetworkClient client)
    {
        base.OnStartClient(client);

        Debug.Log("Client Started! -- " + networkAddress + " :: " + networkPort);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        /// Can't directly send an int variable to 'addPlayer()' so you have to use a message service...
        IntegerMessage msg = new IntegerMessage(avatarIndex);
        /// ***

        // Ready/AddPlayer is usually triggered by a scene load completing. if no scene was loaded, then Ready/AddPlayer it here instead.
        if (!clientLoadedScene)
        {
            Debug.Log("Client AddPlayer!");
            // Call Add player and pass the message
            ClientScene.AddPlayer(conn, 0, msg);
        }

        Debug.Log("Client Connected!");
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // Create message which stores our custom identifier
        IntegerMessage msg = new IntegerMessage(avatarIndex);

        // always become ready.
        ClientScene.Ready(conn);

        bool addPlayer = (ClientScene.localPlayers.Count == 0);
        bool foundPlayer = false;
        for (int i = 0; i < ClientScene.localPlayers.Count; i++)
        {
            if (ClientScene.localPlayers[i].gameObject != null)
            {
                Debug.Log("OnClientSceneChanged:: foundPlayer");
                foundPlayer = true;
                break;
            }
        }
        if (!foundPlayer)
        {
            // there are players, but their game objects have all been deleted
            addPlayer = true;
            Debug.Log("OnClientSceneChanged:: !foundPlayer");
        }
        if (addPlayer)
        {
            Debug.Log("OnClientSceneChanged:: addPlayer");
            // Call Add player and pass the message
            ClientScene.AddPlayer(conn, 0, msg);
        }

        Debug.Log("OnClientSceneChanged:: done");

    }

    /// Copied from Unity's original NetworkManager 'OnServerAddPlayerInternal' script except where noted
    /// Since OnServerAddPlayer calls OnServerAddPlayerInternal and needs to pass the message - just add it all into one.
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {

        /// *** additions
        /// I skipped all the debug messages...
        /// This is added to recieve the message from addPlayer()...
        int id = 0;

        Debug.Log("Adding Player to Server");

        if (extraMessageReader != null) {
            IntegerMessage i = extraMessageReader.ReadMessage<IntegerMessage>();
            id = i.value;
        }

        Debug.Log("id = " + id + " PlayerID = " + playerIDCount);
        
        if(id != -1) // We are picking the player prefab from a list of dragons
        {
            /// using the sent message - pick the correct prefab
            playerPrefab = spawnPrefabs[id];
            /// *** end of additions
        }
        else
        {
            playerPrefab = spawnPrefabs.Find(cam => cam.name.Contains("Fixed"));
        }

        GameObject player;
        Transform startPos = GetStartPosition();
        if (startPos != null)
        {
            player = (GameObject)Instantiate(playerPrefab, startPos.position, startPos.rotation);
        }
        else
        {
            player = (GameObject)Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation);
        }


        //player.GetComponent<NetworkPlayer>().PlayerID = 0;// playerID;

        Debug.Log(playerPrefab.name + " " + id);
        EventManager.instance.TriggerEvent(new EventServerAddedClient(playerPrefab.name + " " + id));

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    #endregion

    #region EventListener

    void Start()
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
            case EventClientAvatarSelected.EventName:
                int eggIndex = (int)evt.GetData();
                avatarIndex = eggIndex;
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
    #endregion
}
