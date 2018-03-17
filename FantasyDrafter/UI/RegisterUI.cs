using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RegisterUI : MonoBehaviour {

    public InputField DisplayName;
    public InputField EmailAddress;
    public InputField Password;

    public void CompleteRegistration()
    {
        new GameSparks.Api.Requests.RegistrationRequest()
          .SetDisplayName(DisplayName.text)
          .SetPassword(Password.text)
          .SetUserName(EmailAddress.text)
          .Send((response) => {
              if (!response.HasErrors)
              {
                  System.Action onUserLoadComplete = delegate
                  {
                      PlayerPrefs.SetString("LastUserEmail", EmailAddress.text);
                      PlayerPrefs.SetString("LastUserPassword", Password.text);

                      Logging.Log("Player Registered…");
                      GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Registration Complete!", "Successfully Registered a new account. You will now return to the main menu!", false);
                      popup.OkButton.onClick.AddListener(() =>
                      {
                          SceneManager.Instance.LoadScene("MainMenu");
                      });
                  };

                  User.ActiveUser = new User(DisplayName.text, EmailAddress.text, onUserLoadComplete);
                  
              }
              else
              {
                  GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Registration Error!", response.Errors.JSON, false);
                  
                  Logging.Log("Error Registering Player: " + response.Errors.ToString());
              }
          }
        );
    }

    public void OnPressAlreadyHaveAccount()
    {
        SceneManager.Instance.LoadScene("SignIn");
    }

}
