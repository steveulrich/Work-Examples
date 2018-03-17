using UnityEngine;
using UnityEngine.UI;

public class SignInUI : MonoBehaviour
{
    public InputField EmailAddress;
    public InputField Password;

    public void OnPressSignIn()
    {
        new GameSparks.Api.Requests.AuthenticationRequest().SetUserName(EmailAddress.text).SetPassword(Password.text).Send((response) => {
            if (!response.HasErrors)
            {
                PlayerPrefs.SetString("LastUserEmail", EmailAddress.text);
                PlayerPrefs.SetString("LastUserPassword", Password.text);

                System.Action onCompleteUserInit = delegate
                {
                    GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Welcome Back!", "Hi, " + response.DisplayName + "! Let's check out your Breakaway Fantasy Team!", false, "Let's Go!");
                    popup.OkButton.onClick.AddListener(() => SceneManager.Instance.LoadScene("MainMenu"));

                    Logging.Log("Player Authenticated SUCCESS...");
                };

                User.ActiveUser = new User(response.DisplayName, EmailAddress.text, onCompleteUserInit);
                
            }
            else
            {
                GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Error!", "Unable to authenticate using those credentials. Please try again or select 'Forgot Password'.", true, "Ok", "Forgot Password");
                popup.CancelButton.onClick.AddListener(() => SceneManager.Instance.LoadScene("ForgotPassword"));
                Logging.Log("Error Authenticating Player...");
            }
        });

    }

    public void OnPressForgotPassword()
    {
        SceneManager.Instance.LoadScene("ForgotPassword");
    }

}
