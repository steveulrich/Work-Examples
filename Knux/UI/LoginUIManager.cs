using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class LoginUIManager : MonoBehaviour
{
    public UnityEngine.UI.InputField DisplayName;
    public UnityEngine.UI.InputField EmailAddress;
    public UnityEngine.UI.InputField PasswordField;

    public void OnClickLogin()
    {
        Debug.Log("Login Button Pressed");
        StartCoroutine(LoginTask());
    }

    IEnumerator LoginTask()
    {
        Debug.Log("----------Login Coroutine Start----------");
        yield return null;

        RegistrationRequest();

        //DeviceAuthRequest();
        //TwitchAuthRequest();
        //SteamAuthRequest();
    }

    void RegistrationRequest()
    {
        new GameSparks.Api.Requests.RegistrationRequest()
            .SetDisplayName(DisplayName.text)
            .SetPassword(PasswordField.text)
            .SetUserName(EmailAddress.text)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Player Registered");

                    new GameSparks.Api.Requests.AuthenticationRequest()
                        .SetUserName(EmailAddress.text)
                        .SetPassword(PasswordField.text)
                        .Send((authResponse) => {
                            if (!authResponse.HasErrors)
                            {
                                Debug.Log("Player Authenticated...");

                                User.ActiveUser = new User(DisplayName.text, EmailAddress.text, PasswordField.text, () =>
                                {
                                    Backend.DynamicProgramProperties.Initialize();
                                    Nux.SceneManager.Instance.LoadScene("MainMenu");// @TODO: Change this to a string with ScenePicker property });
                                });
                            }
                            else
                            {
                                Debug.Log("Error Authenticating Player...");
                            }
                        });
                }
                else
                {
                    Debug.Log("Error Registering Player");
                }
            });
    }

    void DeviceAuthRequest()
    {
        new DeviceAuthenticationRequest()
            .SetDisplayName("Beeven")
            .Send((response) =>
            {
                string authToken = response.AuthToken;
                string displayName = response.DisplayName;
                bool? newPlayer = response.NewPlayer;
                GSData scriptData = response.ScriptData;
                var switchSummary = response.SwitchSummary;
                string userId = response.UserId;

                Debug.Log(authToken + " -- " + displayName + " -- " + newPlayer + " -- " + userId);

                User.ActiveUser = new User("Beeven", userId, string.Empty, () =>
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Bootstrap");// @TODO: Change this to a string with ScenePicker property });
                });
            });
    }

    void TwitchAuthRequest()
    {
        Application.OpenURL("https://api.twitch.tv/kraken/oauth2/authorize");

        new TwitchConnectRequest()
            .SetAccessToken(ApplicationSettings.TWITCH_ACCESS_TOKEN)
            .SetDoNotLinkToCurrentPlayer(false)
            .SetErrorOnSwitch(false)
            .SetSwitchIfPossible(false)
            .SetSyncDisplayName(true)
            .Send((response) =>
            {
                string authToken = response.AuthToken;
                string displayName = response.DisplayName;
                bool? newPlayer = response.NewPlayer;
                GSData scriptData = response.ScriptData;
                var switchSummary = response.SwitchSummary;
                string userId = response.UserId;

                Debug.Log(authToken + " -- " + displayName + " -- " + newPlayer + " -- " + userId);

                UnityEngine.SceneManagement.SceneManager.LoadScene("Bootstrap"); // @TODO: Change this to a string with ScenePicker property
            });
    }

    void SteamAuthRequest()
    {
        new SteamConnectRequest()
        .SetDoNotLinkToCurrentPlayer(false)
        .SetErrorOnSwitch(false)
        .SetSessionTicket(string.Empty)
        .SetSwitchIfPossible(false)
        .SetSyncDisplayName(true)
        .Send((response) =>
        {
            string authToken = response.AuthToken;
            string displayName = response.DisplayName;
            bool? newPlayer = response.NewPlayer;
            GSData scriptData = response.ScriptData;
            var switchSummary = response.SwitchSummary;
            string userId = response.UserId;

            Debug.Log(authToken + " -- " + displayName + " -- " + newPlayer + " -- " + userId);

            UnityEngine.SceneManagement.SceneManager.LoadScene("Bootstrap"); // @TODO: Change this to a string with ScenePicker property
        });
    }
}
