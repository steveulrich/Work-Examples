using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsStreamerAndroid : MonoBehaviour {

    public string serverURL = "http://192.168.1.184:8000";

    void Start () {
#if UNITY_ANDROID
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject contextObj = jc.GetStatic<AndroidJavaObject>("currentActivity");
        
        AndroidJavaObject serverURLJString = new AndroidJavaObject("java.lang.String", serverURL);
        new AndroidJavaObject("com.androidericcson.androidstatstreamer.DemoStatStreamer", contextObj, serverURLJString).Call("Start");
#endif
    }

}
