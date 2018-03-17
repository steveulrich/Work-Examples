using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenUI : MonoBehaviour
{

    public void GoToRegisterAccount()
    {
        SceneManager.Instance.LoadScene("Register");
    }

    public void GoToSignIn()
    {
        SceneManager.Instance.LoadScene("SignIn");
    }

    
}
