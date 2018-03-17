using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Requests;

public class ForgotPasswordUI : MonoBehaviour
{
    public UnityEngine.UI.InputField EmailAddress;
    public UnityEngine.UI.InputField TokenInput;
    public UnityEngine.UI.InputField NewPasswordInput;

    public CanvasGroup ForgotPwdCanvasGroup;
    public CanvasGroup NewPwdCanvasGroup;

    private IEnumerator TransitionBetweenCanvasGroups(CanvasGroup offGroup, CanvasGroup onGroup, float duration = 0.25f)
    {
        offGroup.interactable = false;
        offGroup.blocksRaycasts = false;

        // Fade the offGroup to transparent 
        float counter = 0f;
        while(offGroup.alpha > 0f)
        {
            counter += Time.deltaTime;
            offGroup.alpha = Mathf.Lerp(1f, 0f, counter / duration);
            yield return Backend.Utility.WaitForFrame;
        }

        offGroup.alpha = 0f;
        counter = 0f;

        while (onGroup.alpha < 1f)
        {
            counter += Time.deltaTime;
            offGroup.alpha = Mathf.Lerp(0f, 1f, counter / duration);
            yield return Backend.Utility.WaitForFrame;
        }

        onGroup.interactable = true;
        onGroup.blocksRaycasts = true;
    }

    public void OnSubmitTokenRequest()
    {
        //Construction a GSRquestData object to pass in as scriptData

        GameSparks.Core.GSRequestData script = new GameSparks.Core.GSRequestData();

        script.Add("action", "passwordRecoveryRequest");
        script.Add("email", EmailAddress.text);


        //Sending the request with spaces for Username and Password so no errors are given and scriptData is sent
        //Response is breaken down and the result of the action is determined for debug or feedback to user
        new AuthenticationRequest().SetUserName("").SetPassword("").SetScriptData(script).Send((response) => {

            if (response.HasErrors)
            {
                if (response.Errors.GetString("action") == "complete")
                {
                    GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Email sent!", "Check your email for a reset token.", false);
                    popup.OkButton.onClick.AddListener(() => StartCoroutine(TransitionBetweenCanvasGroups(ForgotPwdCanvasGroup, NewPwdCanvasGroup)));

                    EmailAddress.text = "";
                }
                else
                {
                    Backend.Utility.MakeNewGenericPopup("Email not sent!", "Please ensure email is linked to account", false);
                }
            }
        });
    }

    public void OnSubmitNewPassword()
    {
        //Sending the request with spaces for Username and Password so no errors are given and scriptData is sent
        //Response is breaken down and the result of the action is determined for debug or feedback to user
        GameSparks.Core.GSRequestData script = new GameSparks.Core.GSRequestData();

        script.Add("action", "resetPassword");
        script.Add("token", TokenInput.text);
        script.Add("password", NewPasswordInput.text);

        new AuthenticationRequest().SetUserName("").SetPassword("").SetScriptData(script).Send((response) => {

            if (response.HasErrors)
            {
                if (response.Errors.GetString("action") == "complete")
                {
                    GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Success!", "Password changed. You will now be returned to the main menu.", false);
                    popup.OkButton.onClick.AddListener(() => SceneManager.Instance.LoadScene("TitleScreen"));

                    TokenInput.text = "";
                    NewPasswordInput.text = "";
                }
                else
                {
                    Backend.Utility.MakeNewGenericPopup("Failed!", "Please ensure token is valid", false);
                }
            }
        });
    }
}
