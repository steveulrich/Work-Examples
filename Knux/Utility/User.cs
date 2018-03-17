using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This class describes all the data associated with a User
/// </summary>
public class User
{
    private static User m_activeUser;
    public static User ActiveUser
    {
        get
        {
            if (m_activeUser == null)
            {
                return new User("Test", "Test@test.com", "test", null);
            }
            else
            {
                return m_activeUser;
            }
        }
        set
        {
            m_activeUser = value;
        }
    }

    private string _displayName;
    public string DisplayName
    {
        get { return _displayName; }
    }

    private string _emailAddress;
    public string EmailAddress
    {
        get { return _emailAddress; }
    }

    System.Action callbackOnLoadComplete;

    public User(string displayName, string email, string password, System.Action callbackOnComplete)
    {
        _displayName = displayName;
        _emailAddress = email;

        UnityEngine.PlayerPrefs.SetString("LastUserEmail", email);
        UnityEngine.PlayerPrefs.SetString("LastUserPassword", password);

        callbackOnLoadComplete = callbackOnComplete;
        Load();
    }

    private void Load()
    {
        if (callbackOnLoadComplete != null)
        {
            callbackOnLoadComplete();
        }
    }

}
