using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineupView : MainMenuViewBase, IEventListener
{
    private readonly string EMPTY = "Empty";

    public Sprite DefaultPlayerPortrait;

    public UnityEngine.UI.Text ContestName;
    public UnityEngine.UI.Text RemainingSalaryText;
    public UnityEngine.UI.Text AvgFantasyPointsText;
    public UnityEngine.UI.Button SubmitButton;

    public LineupBuilderPopup LineupPopup;
    public List<FantasyPlayersSlot> FantasyPlayerChoices = new List<FantasyPlayersSlot>();

    public ContestEntry CurrentContestEntry;

    public List<FantasyPlayersSlot> CurrentLineupSlots = new List<FantasyPlayersSlot>();

    private int baseSalary
    {
        get
        {
            int salary = 0;
            if(!Backend.DynamicProgramProperties.TryGetValue("BaseFantasySalary", out salary))
            {
                salary = 30000;
            }

            return salary;
        }
    }

    private float currentTotalFPPG = 0;
    private int fullSlots = 0;
    private int currentEntryIndex;

    private ContestInfo contest;


    // Use this for initialization
    void Start()
    {
        ManageListeners(EventManager.HandleMode.Attach);
    }

    private void OnDestroy()
    {
        ManageListeners(EventManager.HandleMode.Detach);
    }


    public override void Initialize(object[] optionalParams)
    {
        base.Initialize(optionalParams);


        if (optionalParams == null)
        {
            Logging.LogError("Tried to initialize LineupView with optional params but the array was null!");
            return;
        }

        // Try to get a contest from the optional parameters (should never be null)
        contest = (ContestInfo)optionalParams[0];

        if(contest != null)
        {
            ContestName.text = contest.contestTitle;

            int i = 0;

            // Configure the player choices (on the lineup popup) based on the teams in the contest and hte players on those teams
            foreach(var team in contest.Teams)
            {
                foreach(var player in team.Players)
                {
                    FantasyPlayerChoices[i].gameObject.SetActive(true);
                    
                    FantasyPlayerChoices[i].Configure(player);
                    player.OptionSlot = i;

                    FantasyPlayer fPlayer = player; // Avoid Boxing
                    FantasyPlayerChoices[i].AddButton.onClick.RemoveAllListeners();
                    FantasyPlayerChoices[i].AddButton.onClick.AddListener(() => AddFantasyPlayerToEntry(fPlayer));

                    FantasyPlayersSlot slot = FantasyPlayerChoices[i]; // Boxing
                    FantasyPlayerChoices[i].AddButton.onClick.AddListener(() => slot.gameObject.SetActive(false));

                    i++;
                }
            }

            // Turn off the remaining unused choices
            for (; i < FantasyPlayerChoices.Count; i++)
            {
                FantasyPlayerChoices[i].gameObject.SetActive(false);
            }


            for(int index = 0; index < FantasyPlayerChoices.Count; index++)
            {
                FantasyPlayerChoices[index].transform.SetSiblingIndex(index);
            }

            // Configure the current lineup slots with default parameters, including an empty name
            for (int j = 0; j < CurrentLineupSlots.Count; j++)
            {
                CurrentLineupSlots[j].Configure(EMPTY, string.Empty, string.Empty, string.Empty, null);
            }

            CurrentContestEntry.SlotPlayer1.Name = EMPTY;
            CurrentContestEntry.SlotPlayer2.Name = EMPTY;
            CurrentContestEntry.SlotPlayer3.Name = EMPTY;
            CurrentContestEntry.SlotPlayer4.Name = EMPTY;
            CurrentContestEntry.SlotTeam1 = EMPTY;
            CurrentContestEntry.CurrentSalary = 0;
            currentTotalFPPG = 0;
            fullSlots = 0;

            RemainingSalaryText.text = "$" + (baseSalary - CurrentContestEntry.CurrentSalary).ToString();
            AvgFantasyPointsText.text = "0.00";

            SubmitButton.interactable = false;
        }
    }

    public ListenerResult HandleEvent(IEvent evt)
    {
        var evtName = evt.GetName();
        switch (evtName)
        {
            case EventContestEntryPlayerAdded.EventName:
                var addedPlayer = (FantasyPlayer)evt.GetData();

                // Update the player list to find an empty slot and fill it wiht the info of the player that was just selected
                if(addedPlayer != null)
                {
                    fullSlots++;
                    
                    if(fullSlots == Backend.Utility.maxSlotsPerContest)
                    {
                        SubmitButton.interactable = true;
                    }
                    else
                    {
                        SubmitButton.interactable = false;
                    }

                    CurrentContestEntry.CurrentSalary += int.Parse(addedPlayer.Salary);

                    currentTotalFPPG += int.Parse(addedPlayer.Points);

                    float avgFPPG = currentTotalFPPG / fullSlots;

                    AvgFantasyPointsText.text = avgFPPG.ToString("F");
                    
                    RemainingSalaryText.text = "$" + (baseSalary - CurrentContestEntry.CurrentSalary).ToString();

                    FantasyPlayersSlot freeSlot = CurrentLineupSlots[currentEntryIndex];
                    freeSlot.Configure(addedPlayer);
                    freeSlot.AddButton.gameObject.SetActive(false);
                    freeSlot.RemoveButton.gameObject.SetActive(true);

                    freeSlot.RemoveButton.onClick.RemoveAllListeners();
                    int slotIndex = currentEntryIndex; // Boxing
                    freeSlot.RemoveButton.onClick.AddListener(() => OnPressRemovePlayer(slotIndex));
                    freeSlot.RemoveButton.onClick.AddListener( () => RemoveFantasyPlayerFromEntry(addedPlayer));
                }

                break;
            case EventContestEntryPlayerRemoved.EventName:
                var removedPlayer = (FantasyPlayer)evt.GetData();

                // Update the player list to find the removed slot and fill it with empty info
                if(removedPlayer != null)
                {
                    fullSlots--;

                    if (fullSlots == Backend.Utility.maxSlotsPerContest)
                    {
                        SubmitButton.interactable = true;
                    }
                    else
                    {
                        SubmitButton.interactable = false;
                    }

                    currentTotalFPPG -= int.Parse(removedPlayer.Points);

                    float avgFPPG = 0f;

                    if(fullSlots > 0)
                    {
                        avgFPPG = currentTotalFPPG / fullSlots;
                    }
                    else
                    {
                        avgFPPG = 0f;
                    }
                    AvgFantasyPointsText.text = avgFPPG.ToString("F");

                    CurrentContestEntry.CurrentSalary -= int.Parse(removedPlayer.Salary);

                    
                    RemainingSalaryText.text = "$" + (baseSalary - CurrentContestEntry.CurrentSalary).ToString();


                    FantasyPlayersSlot removedSlot = CurrentLineupSlots[currentEntryIndex];
                    removedSlot.Configure(EMPTY, string.Empty, string.Empty, string.Empty, null);
                    removedSlot.RemoveButton.gameObject.SetActive(false);
                    removedSlot.AddButton.gameObject.SetActive(true);

                    FantasyPlayerChoices[removedPlayer.OptionSlot].Configure(removedPlayer);
                    FantasyPlayerChoices[removedPlayer.OptionSlot].gameObject.SetActive(true);
                }

                break;
        }

        return ListenerResult.Cascade;
    }

    public void ManageListeners(EventManager.HandleMode mode)
    {
        if (mode == EventManager.HandleMode.Detach && !EventManager.HasInstance())
        {
            return;
        }

        EventManager.ManageListener(mode, this, EventContestEntryPlayerAdded.EventName);
        EventManager.ManageListener(mode, this, EventContestEntryPlayerRemoved.EventName);
    }

    public void OnPressAddPlayer(int slot)
    {
        currentEntryIndex = slot;
        LineupPopup.enabled = true;
        LineupPopup.gameObject.SetActive(true);
        LineupPopup.SalaryText.text = RemainingSalaryText.text;
    }

    public void OnPressRemovePlayer(int slot)
    {
        currentEntryIndex = slot;
    }

    public void OnPressSubmitLineup()
    {
        User.ActiveUser.AddFantasyContestLineup(CurrentContestEntry.ContestID, CurrentContestEntry.SlotPlayer1.Name,
            CurrentContestEntry.SlotPlayer2.Name, CurrentContestEntry.SlotPlayer3.Name, CurrentContestEntry.SlotPlayer4.Name,
            CurrentContestEntry.SlotTeam1);

        MainMenuUI.Instance.Navigation.ActivateScreen(ViewsNavigation.ScreenID.MyLineups);

        Backend.Utility.MakeNewGenericPopup("Contest Entry Submitted", "Your entry for " + ContestName.text + " has been submitted. You will now be taken to your lineups view. Thanks for entering!", false);

    }

    public void AddFantasyPlayerToEntry(FantasyPlayer player)
    {
        EventManager.instance.TriggerEvent(new EventContestEntryPlayerAdded(player));
    }

    public void RemoveFantasyPlayerFromEntry(FantasyPlayer player)
    {
        EventManager.instance.TriggerEvent(new EventContestEntryPlayerRemoved(player));
    }

}
