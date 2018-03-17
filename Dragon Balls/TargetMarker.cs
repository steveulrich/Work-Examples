using UnityEngine;
using System.Collections;

public class TargetMarker : MonoBehaviour
{
    public int RotationSpeed = 200;

    private Transform m_transform;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
    }

    private void Update()
    {
        m_transform.Rotate(Vector3.up * Time.deltaTime * RotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Dragon"))
        {
            gameObject.SetActive(false);
        }
    }

}
