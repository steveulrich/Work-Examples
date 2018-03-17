using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHeader : MonoBehaviour {

    public UnityEngine.UI.Text DisplayNameText;
    public BackgroundParallaxController Background;

    public void OnPressLeaderboards(UnityEngine.UI.Toggle toggle)
    {
        if (toggle.isOn)
        {
            Background.SetBGTargetPosition(BackgroundParallaxController.RightPosition);
            MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.Leaderboard);
        }
    }

    public void OnPressStandings(UnityEngine.UI.Toggle toggle)
    {
        if (toggle.isOn)
        {
            Background.SetBGTargetPosition(BackgroundParallaxController.LeftPosition);
            MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.Standings);
        }
    }

    public void OnPressMyLineup(UnityEngine.UI.Toggle toggle)
    {
        if (toggle.isOn)
        {
            Background.SetBGTargetPosition(BackgroundParallaxController.ZeroPosition);
            MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.MyLineups);
        }
    }

    public void OnPressContests(UnityEngine.UI.Toggle toggle)
    {
        if (toggle.isOn)
        {
            Background.SetBGTargetPosition(BackgroundParallaxController.ZeroPosition);
            MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.Contests);
        }
    }
}
