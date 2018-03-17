using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Singleton<SceneManager>
{
    public void LoadScene(string scene)
    {
        StartCoroutine(Backend.Utility.LoadScene(scene));
    }
        
}

