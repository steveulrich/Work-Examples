using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallaxController : MonoBehaviour {

    public static Vector2 ZeroPosition = new Vector2(0, -116f);
    public static Vector2 LeftPosition = new Vector2(360f, -116f);
    public static Vector2 RightPosition = new Vector2(-360f, -116f);

    public RectTransform BGRect;

    private Vector2 startingPosition = LeftPosition;
    private Vector2 targetPosition = LeftPosition;

    public float MaxAnimationTime = 1f;
    private float currentAnimationTime;
	
	// Update is called once per frame
	void Update () {

        currentAnimationTime += Time.deltaTime;

        float lerpAmount = currentAnimationTime / MaxAnimationTime;
        BGRect.localPosition = Vector2.Lerp(startingPosition, targetPosition, lerpAmount);

	}

    public void SetBGTargetPosition(Vector2 target)
    {
        targetPosition = target;
        startingPosition = BGRect.localPosition;
        currentAnimationTime = 0f;
    }
}
