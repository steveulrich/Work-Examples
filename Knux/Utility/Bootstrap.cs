using System.Collections;
using UnityEngine;

using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

/// <summary>
/// The bootstrap class handles initialization of various version/binary specific classes throughout the project.
/// 
/// It will be used to do internet connection tests
/// It will be used to load up dynamic properties from our cloud service that can be used by other classes at run-time
/// </summary>
public class Bootstrap : MonoBehaviour {

    public BootstrapLoadingUI BootstrapLoadingCanvas;

    private IEnumerator Start()
    {
        BootstrapLoadingCanvas.LoadingBarImage.fillAmount = 0f;
        bool connectionTestIsComplete = false;
        bool hasConnection = false;
        // Setup a callback that will handle a true or false result from our internet connection test
        System.Action<bool> cbOnConnectionTestComplete = delegate (bool connectionStatus)
        {
            if (connectionStatus)
            {
                Debug.Log("Bootstrap| Connection Test Successful!");
                hasConnection = true;
            }
            else
            {
                Debug.Log("Bootstrap| ***Connection Test NOT Successful!");
                hasConnection = false;
            }

            connectionTestIsComplete = true;
        };

        Debug.Log("Bootstrap| Checking internet connection...");
        // Check for internet connectivity
        yield return Backend.Utility.InternetConnectionTest(cbOnConnectionTestComplete);

        float timer = 0f;
        while (connectionTestIsComplete == false)
        {
            timer += Time.deltaTime;
            if (timer % 1 == 0)
            {
                Debug.Log("Bootstrap| Waiting for Dynamic Properties... " + timer);
            }
            yield return Backend.Utility.WaitForFrame;
        }

        // If we have internet, download a set of dynamic program properties from the cloud
        if (hasConnection)
        {

            yield return MoveStatusBar(0.25f);

            Debug.Log("Bootstrap| Checking Dynamic Properties");


            // @TODO: Get any asset bundles necessary here


            yield return MoveStatusBar(1f);
            yield return new WaitForSeconds(1f);
            Debug.Log("Bootstrap| Load complete");

            // Attempt to login the last user
            // Attempt to login the last user
            if (PlayerPrefs.HasKey("LastUserEmail") && PlayerPrefs.HasKey("LastUserPassword"))
            {
                string email = PlayerPrefs.GetString("LastUserEmail");
                string pwd = PlayerPrefs.GetString("LastUserPassword");
                new GameSparks.Api.Requests.AuthenticationRequest().SetUserName(email).SetPassword(pwd).Send((response) => {
                    if (!response.HasErrors)
                    {
                        System.Action onUserInitialized = delegate
                        {
                            Nux.SceneManager.Instance.LoadScene("MainMenu");

                        };

                        Debug.Log("Bootstrap| Player Authenticated SUCCESS -- " + response.DisplayName + " -- " + email + " -- " + pwd);

                        User.ActiveUser = new User(response.DisplayName, email, pwd, onUserInitialized);

                        Backend.DynamicProgramProperties.Initialize();

                        bool shouldInitSRDebugger = false;
                        Backend.DynamicProgramProperties.TryGetValue<bool>("DebuggerEnabled", out shouldInitSRDebugger);

                        // Enable SR Debugger on development builds ALWAYS
                        if (UnityEngine.Debug.isDebugBuild)
                        {
                            SRDebug.Init();
                        }
                        else // Otherwise check our DP to see if we should enable it for release builds
                        {
                            if (shouldInitSRDebugger)
                            {
                                SRDebug.Init();
                            }
                        }

                    }
                    else
                    {
                        GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Error!", "Unable to relog you in. Press Ok to continue.", false, okButtonCallback: () => { Nux.SceneManager.Instance.LoadScene("Login"); });

                        Debug.Log("Bootstrap| Error Authenticating Player...Sending to Login screen");
                    }
                });
            }
            else
            {
                Debug.Log("Bootstrap| No player info found, loading Login scene...");
                Nux.SceneManager.Instance.LoadScene("Login");
            }
        }
        // If we don't have internet, notify the user that this app requires internet etc etc.
        else
        {
            Backend.Utility.MakeNewGenericPopup("ERROR", "Unable to connect to the internet. Please enable your connection and restart the app", false);
            Debug.Log("Bootstrap| Error, no connection");
        }

        Backend.Utility.CleanMemory();

    }

    private IEnumerator MoveStatusBar(float targetValue)
    {
        float currentFill = BootstrapLoadingCanvas.LoadingBarImage.fillAmount;

        while (BootstrapLoadingCanvas.LoadingBarImage.fillAmount < targetValue)
        {
            currentFill += Time.deltaTime / 2;
            BootstrapLoadingCanvas.LoadingBarImage.fillAmount = Mathf.Lerp(currentFill, targetValue, currentFill / targetValue);

            yield return Backend.Utility.WaitForFrame;
        }
    }
}
