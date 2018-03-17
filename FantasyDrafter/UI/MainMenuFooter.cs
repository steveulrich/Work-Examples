using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFooter : MonoBehaviour {

    private const string BREAKAWAY_HOME_URL = "https://playbreakaway.com";
    private const string NEXT_MATCH = "Next Match - ";
    private const string LIVE = "LIVE!";
    public UnityEngine.UI.Text NextMatchText;

    public GameObject ExtrasUI;

    private bool moreOptionsExpanded = false;

    private void Update()
    {
        if (FantasyManager.Instance.NextMatchDate < System.DateTime.UtcNow)
        {
            // The match is LIVE! Let's maybe reload the user to bootstrap, with bootstrap sending the user to a LIVE MATCH UI while
            // System.DateTime.Now > NextMatchDate, then send them back to bootstrap once
            // the System.DateTime.Now is > NextMatchDate + 2 hours, by then the CurrentWeek should be updated, and boom.
            NextMatchText.text = LIVE;
            return;
        }

        System.TimeSpan timeRemaining = (FantasyManager.Instance.NextMatchDate - System.DateTime.UtcNow);

        int days = (int)timeRemaining.TotalDays;
        int hours = timeRemaining.Hours;
        int minutes = timeRemaining.Minutes;
        int seconds = timeRemaining.Seconds;

        NextMatchText.text = string.Format(NEXT_MATCH + "{0:00}d : {1:00}h : {2:00}m : {3:00}s", days, hours, minutes, seconds);
    }

    public void OnPressExtras()
    {
        ExtrasUI.SetActive(true);
        ExtrasUI.GetComponent<Animator>().Play("ExtrasUIOpen");
    }

    public void OnPressBreakaway()
    {
        GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Play Breakaway", "Press Ok to be taken out of Breakaway Fantasy Draft to the official Breakaway webiste!", true);
        popup.OkButton.onClick.AddListener(() => Application.OpenURL(BREAKAWAY_HOME_URL));
    }

    public void OnPressRules()
    {
        // Show the Rules UI

    }

}
