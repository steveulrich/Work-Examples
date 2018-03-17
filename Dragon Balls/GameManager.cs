using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour, IEventListener
{
    public GameUIManager UIManager;
    public Transform[] StartingPositions;

    public GameObject[] Collectibles;

    public int[] PlayerScores;

    public int WinningScore = 3;

    private void Awake()
    {
        SetCollectibleStatus(false);
    }

    private void InitializeCoinGame()
    {
        Debug.Log("***We're Ready, Initialize Coin Game!");
        // @TODO - Replace UIManager reference with Events
        UIManager.SwitchOffUI();
        //EventManager.instance.TriggerEvent(new EventGameUIStatusChanged());

        //Turn off other NavMeshes so only the game field is Walkable
        SetNavMeshesStatus(false);

        // Turn on CollectibleObjects
        SetCollectibleStatus(true);

        //@TODO - Display the countdown Timer UI
        //EventManager.instance.TriggerEvent(new EventBeginGameCountdownTimer());
    }

    private void DisableCoinGame()
    {
        Debug.Log("Coin game complete. Reset nav meshes and collectibles");

        SetNavMeshesStatus(true);

        SetCollectibleStatus(false);
    }

    private void SetNavMeshesStatus(bool status)
    {
        Debug.Log("Set NavMesh status: " + status);

    }

    private void SetCollectibleStatus(bool status)
    {
        Debug.Log("Set Collectible Status: " + status);

        foreach(var collectible in Collectibles)
        {
            collectible.SetActive(status);
        }
    }

    private void SetWinnerUIStatus(bool status, int playerID)
    {
        Debug.Log("Set Winner UI Status: **" + status + "** for player: **" + playerID + "**" );

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
                InitializeCoinGame();
                break;
            case EventCollectiblePickedUp.EventName:
                int id = (int)evt.GetData();
                PlayerScores[id]++;

                // Check for game over
                if(PlayerScores[id] > WinningScore)
                {
                    // Show the winning player UI
                    SetWinnerUIStatus(true, id);

                    // Reset coin game assets and nav meshes in DisableCoinGame()
                    DisableCoinGame();
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

        EventManager.ManageListener(mode, this, EventGameReadyToStart.EventName);
        EventManager.ManageListener(mode, this, EventCollectiblePickedUp.EventName);
    }
    #endregion

}
