using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MainMenuPlayOptions : MonoBehaviour, IUIDialog
{
    public float TransitionDuration;

    private RectTransform m_rectTransform;

    public IEnumerator MoveDialog(float targetX, float duration, System.Action cbOnFinish = null)
    {
        m_rectTransform.DOAnchorPosX(targetX, duration);

        yield return new WaitForSeconds(duration);

        if(cbOnFinish != null)
        {
            cbOnFinish();
        }

    }

    [ContextMenu("MoveOff")]
    public bool MoveOffScreen()
    {
        m_rectTransform.DOKill();
        StopCoroutine("MoveDialog");


        System.Action onFinish = delegate
        {
            SetStatus(false);
        };

        StartCoroutine(MoveDialog(-m_rectTransform.rect.width, TransitionDuration, onFinish));
        return true;
    }

    [ContextMenu("MoveOn")]
    public bool MoveOnScreen()
    {
        m_rectTransform.DOKill();
        StopCoroutine("MoveDialog");

        SetStatus(true);

        StartCoroutine(MoveDialog(0, 2f));
        return true;
    }

    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
    }

    public void SetStatus(bool status)
    {
        gameObject.SetActive(status);
    }

}
