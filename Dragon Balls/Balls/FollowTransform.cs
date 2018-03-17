using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FollowTransform : NetworkBehaviour {

    public Transform TransformToFollow;
    private Transform m_transform;
    private void Awake()
    {
        m_transform = GetComponent<Transform>();

        if(TransformToFollow == null)
        {
            GameObject foundHead = GameObject.Find("Head");
            if(foundHead != null)
            {
                Debug.Log("FollowTransform| Head!");
                TransformToFollow = foundHead.transform;
            }
            else
            {
                Debug.Log("FollowTransform| Unable to find Head");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(TransformToFollow != null)
        {
            m_transform.position = TransformToFollow.position;
            Vector3 eulerTarget = TransformToFollow.rotation.eulerAngles;
            m_transform.rotation = Quaternion.Euler(0, eulerTarget.y, 0);
        }
    }
}
