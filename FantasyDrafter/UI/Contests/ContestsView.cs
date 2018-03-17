using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class ContestInfo
{
    public string contestID;
    public string contestTitle;
    public Sprite contestLogo;
    public string contestStartDate;

    public List<FantasyTeam> Teams;
}

public class ContestsView : MainMenuViewBase, IEventListener
{
    public Transform ContestsGridParent;

    public static Dictionary<string, ContestInfo> CurrentContests;

    public List<ContestSlot> ContestSlots;

    private void Awake()
    {
        foreach(var slot in ContestSlots)
        {
            slot.gameObject.SetActive(false);
        }

        CurrentContests = new Dictionary<string, ContestInfo>();
    }

    // Use this for initialization
    void Start ()
    {
        ManageListeners(EventManager.HandleMode.Attach);

        GetContestInfo();

	}

    private void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }

    private void GetContestInfo()
    {
        System.Action populateContestSlots = delegate
        {
            if(CurrentContests != null)
            {
                int i = 0;
                foreach (var contest in CurrentContests)
                {

                    ContestSlots[i].ConfigureSlot(true, contest.Value);
                    i++;
                }

                if (i < ContestSlots.Count)
                {
                    for (; i < ContestSlots.Count; i++)
                    {
                        ContestSlots[i].ConfigureSlot(false, null);
                    }
                }
            }

            ContestsGridParent.SetPositionAndRotation(Vector2.zero, Quaternion.identity);

        };

        FantasyUtility.GetContestInfoFromGameSparks(populateContestSlots);
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        string evtName = evt.GetName();

        switch(evtName)
        {
            case EventContestSelected.EventName:
                var contest = (ContestInfo)evt.GetData();

                // Our optionalparams for the lineup screen will have (update this if more options are made)
                // [0] = contestId
                object[] optionalParams = new object[1] { contest };

                // if the user has a lineup for this contest already, notify them they do and take them to view that lineup?
                if(User.ActiveUser.ContestEntries.ContainsKey(contest.contestID))
                {
                    MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.MyLineups, optionalParams);
                }
                else
                {
                    MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.EditLineups, optionalParams);
                }

                return ListenerResult.Consume;
        }

        return ListenerResult.Cascade;
    }

    public void ManageListeners(EventManager.HandleMode mode)
    {
        if (mode == EventManager.HandleMode.Detach && !EventManager.HasInstance())
        {
            return;
        }

        EventManager.ManageListener(mode, this, EventContestSelected.EventName);
    }
}
