using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltOnAxis : MonoBehaviour {

    public Vector3 AxisOfTilt = Vector3.forward;

    public float TiltTarget;
    public float RotationSpeed;

    public float rotationValue;
    private Transform m_transform;

    private void Awake()
    {
        m_transform = GetComponent<Transform>();


    }

    // Update is called once per frame
    void Update ()
    {
        rotationValue = TiltTarget * Mathf.Sin(Time.time * RotationSpeed);
        m_transform.localRotation = Quaternion.Euler(AxisOfTilt * rotationValue);

	}
}
