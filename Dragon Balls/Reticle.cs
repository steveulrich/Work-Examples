using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour
{
    public  bool UseNormal;
    public UnityEngine.UI.Image ReticleImage;
    public Transform Camera;
    public float m_defaultDistance = 5f;

    private Transform m_reticleTransform;

    private Vector3 m_originalScale;
    private Quaternion m_originalRotation;

    private void Awake()
    {
        m_reticleTransform = ReticleImage.GetComponent<Transform>();

        // Store the original scale and rotation.
        m_originalScale = m_reticleTransform.localScale;
        m_originalRotation = m_reticleTransform.localRotation;
    }

    public void Hide()
    {
        ReticleImage.enabled = false;
    }

    public void Show()
    {
        ReticleImage.enabled = true;
    }

    // This overload of SetPosition is used when the the VREyeRaycaster hasn't hit anything.
    public void SetPosition()
    {
        // Set the position of the reticle to the default distance in front of the camera.
        m_reticleTransform.position = Camera.position + Camera.forward * m_defaultDistance;

        // Set the scale based on the original and the distance from the camera.
        m_reticleTransform.localScale = m_originalScale * m_defaultDistance;

        // The rotation should just be the default.
        m_reticleTransform.localRotation = m_originalRotation;
    }

    public void SetPosition(RaycastHit hit)
    {
        m_reticleTransform.position = hit.point;
        m_reticleTransform.localScale = m_originalScale * hit.distance;

        // If the reticle should use the normal of what has been hit...
        if (UseNormal)
            // ... set it's rotation based on it's forward vector facing along the normal.
            m_reticleTransform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
        else
            // However if it isn't using the normal then it's local rotation should be as it was originally.
            m_reticleTransform.localRotation = m_originalRotation;
    }
}
