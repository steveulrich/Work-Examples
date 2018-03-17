using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObscureMe : MonoBehaviour {

    void Start()
    {
        // get all renderers in this object and its children:
        var renders = GetComponentsInChildren<Renderer>();
        foreach (var rendr in renders)
        {
            if(rendr.materials.Length > 1)
            {
                foreach(var mat in rendr.materials)
                {
                    mat.renderQueue = 2002;
                }
            }
            else
            {
                rendr.material.renderQueue = 2002; // set their renderQueue
            }
        }
    }
}
