using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullInspector;

public class ViewsNavigation :  BaseBehavior
{
    public enum ScreenID
    {
        Standings,
        EditLineups,
        MyLineups,
        Leaderboard,
        Contests,
        FantasyPlayers
    }
    public Dictionary<ScreenID, MainMenuViewBase> AllScreens;

    public ScreenID CurrentScreen;

    /// <summary>
    /// Deactivates all screens
    /// </summary>
    public void DeactivateAllScreens()
    {
        foreach (var kvp in AllScreens)
        {
            AllScreens[kvp.Key].Deactivate();
        }
    }

    public void ActivateScreen(ScreenID screen, object[] optionalParams = null)
    {
        if(screen != CurrentScreen)
        {
            AllScreens[CurrentScreen].Deactivate();
            AllScreens[screen].Activate(optionalParams);
            CurrentScreen = screen;
        }
    }
}
