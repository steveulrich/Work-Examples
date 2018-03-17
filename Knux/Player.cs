using System.Collections;
using UnityEngine;
using GameSparks.RT;
using Invector.CharacterController;

public class Player : vMeleeCombatInput
{
    public bool m_isPlayer;
    private Team m_team;

    // Predictive Movement
    private float updateRate = 0.1f;
    public Vector3 GoToPosition;
    public float GoToRotation;
    public float GoToStrafeMagnitude;
    public float GoToDirection;

    private Transform m_transform;

    private WaitForSeconds updateWait;
    private Vector3 m_prevPos;


    private void Awake()
    {
        
        m_transform = this.GetComponent<Transform>();

        updateWait = new WaitForSeconds(updateRate);

    }

    /// <summary>
    /// Takes the details needed to set up each player and to separate the player avatar from
    /// opponent avatars
    /// </summary>
    /// <param name="_spawnPos">The position and rotation of the player's spawn-point</param>
    /// <param name="_isPlayer">If set to <c>true</c> is player.</param>
    /// <param name="_myScoreTxt">this is a reference to the player's score text in the HUD.</param>
    public void SetupPlayerAvatar(Transform spawnPos, bool isPlayer, Team team)
    {
        m_transform.position = spawnPos.position;
        m_transform.rotation = spawnPos.rotation;

        m_isPlayer = isPlayer;

        m_team = team;

        if(isPlayer)
        {
            StartCoroutine(SendPlayerMovement());
        }
        else
        {
            GoToPosition = m_transform.position;
            GoToRotation = m_transform.eulerAngles.z;
        }
    }
    

    private void Update()
    {
        if (!m_isPlayer)
        {
            m_transform.position = Vector3.Lerp(m_transform.position, GoToPosition, Time.deltaTime / updateRate);
            m_transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(m_transform.eulerAngles.y, GoToRotation, Time.deltaTime / updateRate), 0); // lerp the enemy player to the new angle        }
            //this.animator.SetFloat("InputMagnitude", GoToStrafeMagnitude, .2f, Time.deltaTime);
            //this.animator.SetFloat("InputHorizontal", !stopMove && !lockMovement ? GoToDirection : 0f, 0.25f, Time.deltaTime);
        }
    }
    /// <summary>
    /// Sends the player position, rotation and velocity
    /// </summary>
    /// <returns>The player movement.</returns>
    private IEnumerator SendPlayerMovement()
    {
        // we don't want to send position updates until we are actually moving //
        // so we check that the axis-input values are greater or less than zero before sending //
        if ((this.transform.position != m_prevPos) || (Mathf.Abs(Input.GetAxis("Vertical")) > 0) || (Mathf.Abs(Input.GetAxis("Horizontal")) > 0))
        {
            using (RTData data = RTData.Get())
            {  
                // we put a using statement here so that we can dispose of the RTData objects once the packet is sent

                // Send Position Data
                data.SetVector3(1, new Vector3(m_transform.position.x, m_transform.position.y, m_transform.position.z)); // add the position at key 1

                // Send Rotation Data
                data.SetFloat(2, m_transform.eulerAngles.y); // add the rotation at key 2

                //data.SetVector2(3, new Vector2(this.strafeMagnitude, this.direction));

                GameSparksManager.Instance().GetRTSession().SendData(2, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);// send the data

            }
            m_prevPos = this.transform.position; // record position for any discrepancies
        }

        yield return updateWait;

        StartCoroutine(SendPlayerMovement());
    }
}
