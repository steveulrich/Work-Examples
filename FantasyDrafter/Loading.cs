using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour {

    public static UnityEngine.UI.Image LoadingGraphic;

    private void Awake()
    {
        LoadingGraphic = GetComponentInChildren<UnityEngine.UI.Image>();
    }

    public static IEnumerator FadeToClear(float duration = 0.5f)
    {
        yield return FadeLoading(duration, 0f);
    }

    public static IEnumerator FadeToOpaque(float duration = 0.5f)
    {
        yield return FadeLoading(duration, 1f);
    }

    private static IEnumerator FadeLoading(float duration, float targetAlpha)
    {
        if(LoadingGraphic != null)
        {
            LoadingGraphic.raycastTarget = true;
            Color targetColor = new Color(LoadingGraphic.color.r, LoadingGraphic.color.g, LoadingGraphic.color.b, targetAlpha);
            Color baseColor = LoadingGraphic.color;
            float currentTimer = 0f;
            while (currentTimer <= duration)
            {
                LoadingGraphic.color = Color.Lerp(baseColor, targetColor, currentTimer / duration);
                currentTimer += Time.deltaTime;
                yield return Backend.Utility.WaitForFrame;
            }

            LoadingGraphic.color = targetColor;
            LoadingGraphic.raycastTarget = false;
        }
    }
}
