using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FantasyDownloadManager : MonoBehaviour {

    private static float _progress;
    public static float Progress
    {
        get { return _progress; }
    }


    public static void DownloadFantasyManager()
    {
        _progress = 0f;
    }
}
