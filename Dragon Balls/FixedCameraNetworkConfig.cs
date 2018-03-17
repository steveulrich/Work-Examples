using UnityEngine;
using UnityEngine.Networking;

public class FixedCameraNetworkConfig : NetworkBehaviour
{
    Camera PlayerCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        PlayerCamera = Camera.main;

        PlayerCamera.transform.SetParent(transform);
        PlayerCamera.transform.localPosition = Vector3.zero;
        PlayerCamera.transform.localRotation = Quaternion.identity;
    }

}
