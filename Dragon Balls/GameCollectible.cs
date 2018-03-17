using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameCollectible : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Dragon"))
        {
            // Get the dragon's player ID
            int id = 0;//@TODO other.GetComponent<DragonController>().playerID;

            Debug.Log("Collectible gathered by: " + id); // @TODO: Add player ID to the log message

            //  Send an event (? or command) to the server that the collectible has been picked up.
            ServerManager.Instance.RpcCollectiblePickedUp(id);

            // Disable this object
            ServerManager.Instance.RpcDisableGameCollectible(this.gameObject);
        }
    }

    
}
