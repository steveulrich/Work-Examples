using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkMenuUI : MonoBehaviour
{
    public UnityEngine.UI.InputField NetworkAddress;
    public UnityEngine.UI.InputField NetworkPort;
    public UnityEngine.UI.InputField RelocURL;

    public GameObject SvrCamera;
    public GameObject MainCamera;

    public UnityEngine.UI.Text IPAddressText;

    [SceneSelector] public string DemoConfigScene;
    [SceneSelector] public string LobbyScene;
    [SceneSelector] public string GameScene;
    [SceneSelector] public string FixedCamUIConfigScene;
    [SceneSelector] public string ServerInfoScene;

    public void Start()
    {
        IPAddressText.text = Network.player.ipAddress; // Text to display user's IP address
        NetworkAddress.text = "10.47.108.171"; //InputField for network address

#if UNITY_STANDALONE && !UNITY_EDITOR

        SvrCamera.SetActive(false);

        MainCamera.SetActive(true);

#endif
    }

    public void OnSelectDragonTamer()
    {
        SetNetworkConfig();

        EventManager.instance.TriggerEvent(new EventClientAvatarSelected(0));
        DragonNetworkManager.singleton.playerPrefab = DragonNetworkManager.singleton.spawnPrefabs[0];

        // This will be a local player, so load the "Lobby" scene
        DragonNetworkManager.singleton.StartClient();

        //UnityEngine.SceneManagement.SceneManager.LoadScene(LobbyScene);
    }

    public void OnSelectServer()
    {
        SetNetworkConfig();

        SvrCamera.SetActive(true);

        DragonNetworkManager.singleton.StartServer();

        UnityEngine.SceneManagement.SceneManager.LoadScene(ServerInfoScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);

    }

    private void Update() {
        //if(Input.GetKey(KeyCode.S)) {
        //    NetworkAddress.text = "localhost";
        //    RelocURL.text = "localhost";
        //    OnSelectServer();
        //}
        //if (Input.GetKey(KeyCode.C)) {
        //    OnSelectDragonTamer();
        //}
        //if (Input.GetKey(KeyCode.F)) {
        //    OnSelectFixedCamera();
        //}
    }


    public void OnSelectFixedCamera()
    {
        SetNetworkConfig();

        EventManager.instance.TriggerEvent(new EventClientAvatarSelected(1));
        DragonNetworkManager.singleton.playerPrefab = DragonNetworkManager.singleton.spawnPrefabs[1];

        Debug.Log("NetworkMenuUI| playerPrefab = " + DragonNetworkManager.singleton.playerPrefab.gameObject.name);

        DragonNetworkManager.singleton.StartClient();
        
        //Load into the Game scene directly, ensure the player is configured as a FixedCamera
        //UnityEngine.SceneManagement.SceneManager.LoadScene(FixedCamUIConfigScene);
    }

    private void SetNetworkConfig()
    {
        DragonNetworkManager.singleton.networkAddress = NetworkAddress.text;
        DragonNetworkManager.singleton.networkPort = int.Parse(NetworkPort.text);
        DragonNetworkManager.RelocURL = RelocURL.text;
    }
}
