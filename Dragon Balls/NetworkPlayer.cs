using UnityEngine;
using UnityEngine.Networking;
using MalbersAnimations;

public class NetworkPlayer : NetworkBehaviour, IEventListener
{
    public int PlayerID;
    public Transform CameraParent;
    public DragonHandler Avatar;

    public DragonHandler spawnedAvatar;

    public Transform MainCam;
    

    void SpawnAvatar()
    {
        CmdSpawnAvatar();
    }

    [Command]
    void CmdSpawnAvatar()
    {
        Debug.Log("Server received command: Spawn Avatar -- ID: " + PlayerID);

        DragonSpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<DragonSpawnPoint>();

        Transform spawnPointToUse = null;

        foreach(DragonSpawnPoint spawn in spawnPoints)
        {
            if(spawn.SpawnPointID == PlayerID)
            {
                spawnPointToUse = spawn.transform;
            }
        }

        DragonHandler avatarInstance = Instantiate(Avatar, spawnPointToUse.position, Quaternion.identity) as DragonHandler;

        NetworkServer.SpawnWithClientAuthority(avatarInstance.gameObject, connectionToClient);

        RpcSetAvatar(avatarInstance.gameObject);
    }

    [ClientRpc]
    void RpcSetAvatar(GameObject avatar)
    {
        Debug.Log("Client received RPC: Spawn Avatar");
        spawnedAvatar = avatar.GetComponent<DragonHandler>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        MainCam = GameObject.Find("SvrCamera").transform;
        transform.SetParent(MainCam);

        //transform.SetParent(MainCam.GetChild(0));
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        //ClientScene.RegisterPrefab(Avatar.gameObject);
        //SpawnAvatar();
    }
    #region BallBunch Client/Server
    [Command]
    void CmdPressBallBunch(int handlerIndex)
    {
        EventManager.instance.TriggerEvent(new EventRequestBallBunchRelease(handlerIndex));
        RpcReleaseBallBunch(handlerIndex);

    }

    [ClientRpc]
    void RpcReleaseBallBunch(int handlerIndex)
    {
        EventManager.instance.TriggerEvent(new EventRequestBallBunchRelease(handlerIndex));
    }

    [Command]
    void CmdResetBallBunch()
    {
        EventManager.instance.TriggerEvent(new EventBallBunchResetRequested());
        RpcResetBallBunch();

    }

    [ClientRpc]
    void RpcResetBallBunch()
    {
        EventManager.instance.TriggerEvent(new EventBallBunchResetRequested());
    }

    #endregion

    #region CannonGame Client/Server

    [Command]
    void CmdStartCannonGame()
    {
        EventManager.instance.TriggerEvent(new EventCannonGameStart());
        RpcStartCannonGame();
    }

    [ClientRpc]
    void RpcStartCannonGame()
    {
        EventManager.instance.TriggerEvent(new EventCannonGameStart());
    }

    #endregion

    #region Fireplace Client/Server

    [Command]
    void CmdStartFire()
    {
        EventManager.instance.TriggerEvent(new EventStartFire());
        RpcStartFire();
    }

    [ClientRpc]
    void RpcStartFire()
    {
        EventManager.instance.TriggerEvent(new EventStartFire());
    }

    #endregion

    private void OnRoomScan()
    {

    }

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
            case EventBallBunchPressed.EventName:
                BallReleaseHandler bunchRelease = (BallReleaseHandler)evt.GetData();

                if(bunchRelease != null)
                {
                    int bunchIndex = bunchRelease.BallBunchIndex;
                    //Debug.Log("BallRelease| " + bunchIndex);
                    CmdPressBallBunch(bunchIndex);
                }
                break;
            case EventBallBunchResetPressed.EventName:
                CmdResetBallBunch();
                break;
            case EventCannonGameRequested.EventName:
                CmdStartCannonGame();
                break;
            case EventRequestStartFire.EventName:
                CmdStartFire();
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

        EventManager.ManageListener(mode, this, EventBallBunchPressed.EventName);
        EventManager.ManageListener(mode, this, EventBallBunchResetPressed.EventName);
        EventManager.ManageListener(mode, this, EventCannonGameRequested.EventName);
        EventManager.ManageListener(mode, this, EventRequestStartFire.EventName);
    }

}
