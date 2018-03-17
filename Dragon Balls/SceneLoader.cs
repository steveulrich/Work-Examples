using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour {

    [SceneSelector]
    public string SceneToLoad1;
    [SceneSelector]
    public string SceneToLoad2;

    // Use this for initialization
    IEnumerator Start ()
    {
        SvrManager.Instance.SetOverlayFade(SvrManager.eFadeState.FadeOut);

        yield return new WaitForSeconds(1.5f);

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneToLoad1, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneToLoad2, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        SvrManager.Instance.SetOverlayFade(SvrManager.eFadeState.FadeIn);
    }

}
