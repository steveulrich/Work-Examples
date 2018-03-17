using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GenericPopup : MonoBehaviour
{
    public Animator Anim;

    public Text TitleText;
    public Text MessageText;

    public Button CancelButton;
    public Text CancelButtonText;
    public Button OkButton;
    public Text OkButtonText;

    private int closePopupAnimHash;

    private void OnEnable()
    {
        closePopupAnimHash = Animator.StringToHash("ClosePopup");
    }

    public void ClosePopup()
    {
        Anim.Play(closePopupAnimHash);
    }

    public void DestroyPopup()
    {
        Destroy(this.gameObject);
    }

}
