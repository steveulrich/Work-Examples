using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AvatarGazeControl : NetworkBehaviour
{
    public NetworkPlayer Player;

    public Transform TargetMarker;

    public Transform Camera;

    [TagSelector, SerializeField] private string FloorTag;
    [TagSelector, SerializeField] private string WallTag;

    [SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
    [SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
    [SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
    [SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
    [SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
    [SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

    private void Awake()
    {
        if (Player == null)
        {
            Player = GetComponent<NetworkPlayer>();
        }

        TargetMarker.rotation = Quaternion.identity;

        m_Reticle = GameObject.FindObjectOfType<Reticle>();

        TargetMarker.transform.SetParent(null);
    }

    private void OnDestroy()
    {
        if(TargetMarker.gameObject != null)
            Destroy(TargetMarker.gameObject);
    }

    #region Reticle
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        EyeRaycast();
    }

    private void EyeRaycast()
    {
        // Show the debug ray if required
        if (m_ShowDebugRay)
        {
            Debug.DrawRay(Camera.position, Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
        }

        // Create a ray that points forwards from the camera.
        Ray ray = new Ray(Camera.position, Camera.forward);
        RaycastHit hit;

        // Do the raycast forweards to see if we hit anything in the scene
        if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
        {
            // Something was hit, set at the hit position.
            if (m_Reticle)
                m_Reticle.SetPosition(hit);

            if (hit.collider.CompareTag(FloorTag) || hit.collider.CompareTag(WallTag))
            {
                m_Reticle.ReticleImage.color = Color.green;

                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.CompareTag(FloorTag))
                    {
                        // We hit the floor, and would like to move 
                        SelectMoveTarget(hit.point);
                    }
                    else // No need for another walltag comparison since we already know it's either floor or wall and not floor
                    {
                        // Get a ray that shoots from the hit point on the wall, downward (this should hit the floor)
                        Ray wallRay = new Ray(hit.point, hit.collider.transform.TransformDirection(Vector3.down));
                        RaycastHit wallHit;
                        if (Physics.Raycast(wallRay, out wallHit, float.MaxValue))
                        {
                            if (wallHit.collider.CompareTag(FloorTag))
                            {
                                // We hit the floor, and would like to move 
                                SelectMoveTarget(wallHit.point);
                            }
                        }
                    }
                }
            }
            else
            {
                m_Reticle.ReticleImage.color = Color.red;
            }
        }
        else
        {
            // Position the reticle at default distance.
            if (m_Reticle)
                m_Reticle.SetPosition();
        }
    }
    #endregion

    /// <summary>
    /// Take the given hit point and move the avatar as well as position the target marker to that point
    /// </summary>
    /// <param name="hitPoint">Gained from the point property of a RaycastHit</param>
    void SelectMoveTarget(Vector3 hitPoint)
    {
        StopAllCoroutines();

        TargetMarker.position = hitPoint;
        TargetMarker.gameObject.SetActive(true);

        Renderer avatarRenderer = Player.spawnedAvatar.GetComponentInChildren<Renderer>();
        if (avatarRenderer.isVisible)
        {
            Player.spawnedAvatar.SetDestination(hitPoint);
        }
        else
        {
            StartCoroutine(WaitForVisibleToMove(avatarRenderer, hitPoint));
        }
                
    }

    IEnumerator WaitForVisibleToMove(Renderer avatarRenderer, Vector3 hitPoint)
    {
        while(avatarRenderer.isVisible == false)
        {
            yield return null;
        }

        Player.spawnedAvatar.SetDestination(hitPoint);
    }



    
}