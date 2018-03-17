using UnityEngine;
using System.Collections;

public class FixedCameraConfigurationUI : MonoBehaviour
{
    public GameObject FixedCamera1;
    public GameObject FixedCamera2;
    
    public void OnSelectCamera1()
    {
        DragonNetworkManager.singleton.playerPrefab = FixedCamera1;
        DragonNetworkManager.singleton.StartClient();
    }

    public void OnSelectCamera2()
    {
        DragonNetworkManager.singleton.playerPrefab = FixedCamera2;
        DragonNetworkManager.singleton.StartClient();
    }

}
