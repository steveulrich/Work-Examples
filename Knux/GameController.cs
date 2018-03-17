using System.Collections;
using System.Collections.Generic;
using GameSparks.RT;
using UnityEngine;

public enum Team
{
    Red,
    Blue
    //Spectator
}

public class GameController : MonoBehaviour {

    private static GameController m_instance;
    public static GameController Instance
    {
        get { return m_instance; }
    }

    public vThirdPersonCamera ThirdPersonCamera;

    public GameObject PlayerAvatarPrefab;
    public Color BlueTeamMat;
    public Color RedTeamMat;

    private Player[] m_playerAvatarList;



    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void Start()
    {
        PlayerSpawnPoint[] allSpawnPoints = GetAllSpawnPoints();

        SetupAllPlayers(allSpawnPoints);

        if(ThirdPersonCamera == null)
        {
            Debug.Log("GC| Forgot to set the reference to ThirdPersonCamera. Don't sweat it, we'll do some expensive shit to make up for your mistake");
            ThirdPersonCamera = FindObjectOfType<vThirdPersonCamera>();
        }
    }

    private PlayerSpawnPoint[] GetAllSpawnPoints()
    {
        PlayerSpawnPoint[] spawnPoints = FindObjectsOfType(typeof(PlayerSpawnPoint)) as PlayerSpawnPoint[];

        return spawnPoints;

    }

    private void SetupAllPlayers(PlayerSpawnPoint[] spawnPoints)
    {
        int playerCount = 1;
        if(GameSparksManager.Instance())
        {
            playerCount = (int)GameSparksManager.Instance().GetSessionInfo().GetPlayerList().Count;
        }

        m_playerAvatarList = new Player[playerCount];

        Debug.Log("GC| Found " + m_playerAvatarList.Length + " Players...");

        // Loop through each player, and all spawn points, to assign each player to a spawn point based on the player's peerID
        for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
        {
            Debug.Log("GameController| playerIndex:" + playerIndex);
            for (int spawnerIndex = 0; spawnerIndex < spawnPoints.Length; spawnerIndex++)
            {
                Debug.Log("    GameController| Peer ID: " + GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[playerIndex].peerID + " || SpawnID: " + spawnPoints[spawnerIndex].SpawnID);
                if (spawnPoints[spawnerIndex].SpawnID == GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[playerIndex].peerID)
                {
                    GameObject newAvatar = Instantiate(PlayerAvatarPrefab, spawnPoints[spawnerIndex].gameObject.transform.position, spawnPoints[spawnerIndex].gameObject.transform.rotation) as GameObject;
                    newAvatar.name = GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[playerIndex].peerID.ToString();

                    m_playerAvatarList[playerIndex] = newAvatar.GetComponent<Player>(); // add the new tank object to the corresponding reference in the list

                    if (GameSparksManager.Instance().GetSessionInfo().GetPlayerList()[playerIndex].peerID == GameSparksManager.Instance().GetRTSession().PeerId)
                    {
                        m_playerAvatarList[playerIndex].SetupPlayerAvatar(spawnPoints[spawnerIndex].gameObject.transform, true, Team.Blue); // @TODO - Setup HUD, setup player team to be read
                        ThirdPersonCamera.SetMainTarget(newAvatar.transform);
                    }
                    else
                    {
                        m_playerAvatarList[playerIndex].SetupPlayerAvatar(spawnPoints[spawnerIndex].gameObject.transform, false, Team.Red); // @TODO - Setup HUD, setup player team to be read
                    }

                    break;
                }
            }
        }
    }


    /// <summary>
    /// Updates the opponents' position, rotation, and if they have been reset
    /// </summary>
    /// <param name="_packet">Packet Received From Opponent Player</param>
    public void UpdateOpponents(RTPacket _packet)
    {
        Debug.Log(_packet.ToString());

        for (int i = 0; i < m_playerAvatarList.Length; i++)
        {
            if (m_playerAvatarList[i].name == _packet.Sender.ToString())
            { // check the name of the player matches the sender
                Debug.Log("GC| Packet sender verified: " + m_playerAvatarList[i].name);
                // we calculate the new position the player should go to be the position they are at plus the velocity. That is, their position plus the distance they traveled according to their last speed
                // 1 (Position)
                m_playerAvatarList[i].GoToPosition = (new Vector3(_packet.Data.GetVector3(1).Value.x, _packet.Data.GetVector3(1).Value.y, _packet.Data.GetVector3(1).Value.z));// + (new Vector2(_packet.Data.GetVector4(1).Value.z, _packet.Data.GetVector4(1).Value.w));
                // 2 (Rotation)
                m_playerAvatarList[i].GoToRotation = _packet.Data.GetFloat(2).Value;
                // 3 (Animator vars)
                m_playerAvatarList[i].GoToStrafeMagnitude = _packet.Data.GetVector2(3).Value.x;
                m_playerAvatarList[i].GoToDirection = _packet.Data.GetVector2(3).Value.y;
                break; // break, because we don’t need to update any other players.
            }
        }
    }
    /// <summary>
    /// Instantiates the opponent projectiles with the id of the opponent
    /// </summary>
    /// <param name="_packet">Packet Received From Opponent Player</param>
    public void InstantiateOpponentProjetile(RTPacket _packet)
    {

    }
    /// <summary>
    /// Updates the opponent projectiles's rotation and position
    /// </summary>
    /// <param name="_packet">Packet Received From Opponent Player</param>
    public void UpdateOpponentProjectiles(RTPacket _packet)
    {

    }
    /// <summary>
    /// This is called when an opponent has registered a collision.
    /// It will remove the projectile that hit, reset the opponent's player, and update the
    /// score of the owner of the projectile that hit.
    /// </summary>
    /// <param name="_packet">Packet Received From Opponent Player</param>
    public void RegisterOpponentCollision(RTPacket _packet)
    {

    }
    /// <summary>
    /// This method is called when a player disconnects from the RT session
    /// </summary>
    /// <param name="_peerId">Peer Id of the player who disconnected</param>
    public void OnOpponentDisconnected(int _peerId)
    {

    }
}
