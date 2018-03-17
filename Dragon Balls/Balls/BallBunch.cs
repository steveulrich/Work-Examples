using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBunch : MonoBehaviour
{
    public float ForceDelayTime;
    public float DragDelayTime = 4f;
    public int ForceMultiplier = 25;
    public Vector3 ForceAxis = -Vector3.forward;

    Rigidbody[] m_balls;
    Vector3[] ballsOrigin;

    private void Awake()
    {
        m_balls = GetComponentsInChildren<Rigidbody>();
        ballsOrigin = new Vector3[m_balls.Length];

        for(int i = 0; i < m_balls.Length; i++)
        {
            var body = m_balls[i];
            body.useGravity = false;
            body.isKinematic = true;

            ballsOrigin[i] = body.transform.localPosition;
            
        }
    }

    [ContextMenu("Release Balls")]
    public void ReleaseBalls()
    {
        foreach(var body in m_balls)
        {
            body.useGravity = true;
            body.isKinematic = false;
            body.drag = 0f;
        }

        StartCoroutine(AddBallForce());
    }

    IEnumerator AddBallForce()
    {
        yield return new WaitForSeconds(ForceDelayTime);

        foreach (var body in m_balls)
        {
            body.AddForce(ForceAxis * ForceMultiplier);

        }
        yield return new WaitForSeconds(DragDelayTime);

        foreach (var body in m_balls)
        {
            body.drag = 5f;
        }
    }

    public void ResetRequest()
    {
        EventManager.instance.TriggerEvent(new EventBallBunchResetPressed());
    }

    [ContextMenu("Reset Balls")]
    public void ResetBalls()
    {
        StopCoroutine(AddBallForce());
        for (int i = 0; i < m_balls.Length; i++)
        {
            var body = m_balls[i];
            body.useGravity = false;
            body.isKinematic = true;
            body.transform.localPosition = ballsOrigin[i];
            body.drag = 0f;
        }
    }
}
