using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingsView : MainMenuViewBase
{
    public List<StandingsSlot> StandingsSlots = new List<StandingsSlot>();
	// Use this for initialization
	void Start ()
    {
        //SetupUI();
        //Activate();	
	}

    /// <summary>
    /// Get a list of FantasyTeams from teh FantasyManager's standings dictionary
    /// </summary>
    /// <returns>A list containing FantasyTeam objects for each team</returns>
    List<FantasyTeam> GetFantasyTeamList()
    {
        List<FantasyTeam> Teams = new List<FantasyTeam>();

        foreach(var team in FantasyManager.Instance.StandingsDictionary)
        {
            Teams.Add(team.Value);
        }

        return Teams;
    }

    void SetupUI()
    {
        FantasyTeamComparer comparer = new FantasyTeamComparer();

        List<FantasyTeam> teamList = GetFantasyTeamList();

        teamList.Sort(comparer);
        if(teamList.Count == StandingsSlots.Count)
        {
            for (int i = 0; i < StandingsSlots.Count; i++)
            {
                StandingsSlots[i].RankText.text = teamList[i].Salary.ToString();
                StandingsSlots[i].RecordText.text = teamList[i].FPPG;
                StandingsSlots[i].TeamNameText.text = teamList[i].TeamName;
            }
        }
        else
        {
            GenericPopup popup = Backend.Utility.MakeNewGenericPopup("Uh Oh!", "Something went wrong trying to populate the standings list. Try to restart your application. If the problem persists, contact me at realbuttabuttajam@gmail.com. Press Ok to quit the application.", false);
            popup.OkButton.onClick.AddListener(() => Application.Quit());
        }
        
    }
	
}
