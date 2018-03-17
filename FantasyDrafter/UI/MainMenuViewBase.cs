using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuViewBase : MonoBehaviour
{
    public DG.Tweening.DOTweenAnimation TweenAnimation;

    public virtual void Activate(object[] optionalParams = null)
    {
        gameObject.SetActive(true);

        if (optionalParams != null)
        {
            Initialize(optionalParams);
        }
        else
        {
            Initialize();
        }

        MoveOnScreen();
    }

    public void Deactivate()
    {
        MoveOffScreen();
    }

    public virtual void Initialize()
    {

    }

    public virtual void Initialize(object[] optionalParams)
    {

    }

    public void MoveOffScreen()
    {
        TweenAnimation.DORestartById("OffScreen");
    }

    public void MoveOnScreen()
    {
        TweenAnimation.DORestartById("OnScreen");
    }
	
}
