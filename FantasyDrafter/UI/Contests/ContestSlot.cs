using UnityEngine;

public class ContestSlot : MonoBehaviour
{

    private const string LIVE = "Live!";
    private const string STARTS_IN = "Starts in - ";
    public ContestInfo Contest;
    public UnityEngine.UI.Text ContestTitle;
    public UnityEngine.UI.Image ContestLogo;
    public UnityEngine.UI.Text ContestStartText;

    public System.DateTime StartDate;

    public void ConfigureSlot(bool isActive, ContestInfo contestInfo)
    {
        this.gameObject.SetActive(isActive);

        if(isActive)
        {
            Contest = contestInfo;
            ContestTitle.text = contestInfo.contestTitle;
            ContestLogo.sprite = contestInfo.contestLogo;

            StartDate = Backend.Utility.ConvertTimestampToDatetime(contestInfo.contestStartDate);
        }
    }

    public void SelectContest()
    {
        EventManager.instance.TriggerEvent(new EventContestSelected(Contest));
    }

    private void Update()
    {
        if (StartDate < System.DateTime.UtcNow)
        {
            // The match is LIVE!
            ContestStartText.text = LIVE;
            this.GetComponent<UnityEngine.UI.Button>().interactable = false;
            this.enabled = false;
            return;
        }

        System.TimeSpan timeRemaining = (StartDate - System.DateTime.UtcNow);

        int days = (int)timeRemaining.TotalDays;
        int hours = timeRemaining.Hours;
        int minutes = timeRemaining.Minutes;
        int seconds = timeRemaining.Seconds;

        ContestStartText.text = string.Format("{0:00}d : {1:00}h : {2:00}m : {3:00}s", days, hours, minutes, seconds);
    }
}
