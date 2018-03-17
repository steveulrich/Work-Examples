using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    public UnityEngine.UI.Text PlayerNameText;

    public MainMenuUINavigation MainMenuUINavigation;

    private RTSessionInfo tempRTSessionInfo;

    private void Start()
    {
        PlayerNameText.text = User.ActiveUser.DisplayName;

        GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;
        SetMatchmakingErrorListener();
    }

    private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message)
    {
        Debug.Log("GameSparks| Match Found!...");

        System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();
        sBuilder.AppendLine("Opponents:" + _message.Participants.Count());
        sBuilder.AppendLine("_________________");
        sBuilder.AppendLine(); // we'll leave a space between the player-list and the match data
        foreach (GameSparks.Api.Messages.MatchFoundMessage._Participant player in _message.Participants)
        {
            sBuilder.AppendLine("Player:" + player.PeerId + " User Name:" + player.DisplayName); // add the player number and the display name to the list
        }

        tempRTSessionInfo = new RTSessionInfo(_message);

        Backend.Utility.MakeNewGenericPopup("Match Found!", sBuilder.ToString(), false);

        // TEMP @TODO: Read ReadyForMatch summary
        Invoke("ReadyForMatch", 2f);
    }

    private void SetMatchmakingErrorListener()
    {
        // Add a listener to GameSparksMessages for a match not being found
        GameSparks.Api.Messages.MatchNotFoundMessage.Listener = (message) => {
            Backend.Utility.MakeNewGenericPopup("Match Not Found!", message.JSONString, false);
        };
    }

    /// <summary>
    /// Called when a player accepts the Match Found dialog box. Should send that they are ready for the match to start, once
    /// all players ready up, transition everyone to the TestLevel scene (make them load the scene async first, then when everyone
    /// is loaded, activate the scene on all clients. Once all scenes are activated, sync timers and begin countdown
    /// </summary>
    private void ReadyForMatch()
    {
        GameSparksManager.Instance().StartNewRTSession(tempRTSessionInfo);
        //Nux.SceneManager.Instance.LoadScene("TestLevel");
    }

    #region ButtonPressCallbacks

    //public float ease = 1f;
    //public Ease easMode;
    public void OnPressPlay()
    {
        Debug.Log("MainMenu| Play Button Pressed");

        // Animate in the PlayOptions panel
        EventManager.instance.TriggerEvent(new EventChangeMainMenuDialog(MainMenuUINavigation.MainMenuDialogs.PlayOptions));

    }

    public void OnPressCustomize()
    {
        Debug.Log("MainMenu| Customize Button Pressed");
    }

    public void OnPressStore()
    {
        Debug.Log("MainMenu| Store Button Pressed");
    }

    public void OnPressLeaderboards()
    {
        Debug.Log("MainMenu| Leaderboards Button Pressed");
    }

    public void OnPressUnranked()
    {
        Debug.Log("MainMenu| Casual Queue Button Pressed");

        // Enable/Animate Unranked Animation Screen
        GameSparksManager.Instance().FindPlayers("DefaultMatch");
    }

    public void OnPressRanked()
    {
        Debug.Log("MainMenu| Ranked Queue Button Pressed");
    }

    public void OnPressTournament()
    {
        Debug.Log("MainMenu| Tournament Queue Button Pressed");
    }

    public void OnPressCustom()
    {
        Debug.Log("MainMenu| Custom Button Pressed");
    }

    public void OnPressSettings()
    {
        Debug.Log("MainMenu| Custom Button Pressed");
    }

    public void OnPressQuit()
    {
        Debug.Log("MainMenu| Quit Button Pressed");

        Application.Quit();
    }

    public void OnPressParty1()
    {
        Debug.Log("MainMenu| Party1 Button Pressed");

    }

    public void OnPressParty2()
    {
        Debug.Log("MainMenu| Party2 Button Pressed");
    }

    public void OnPressAccount()
    {
        Debug.Log("MainMenu| Account Button Pressed");
    }

    #endregion
}
