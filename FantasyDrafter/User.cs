using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This class describes all the data associated with a User
/// </summary>
public class User
{
    public static User ActiveUser;

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

    // Dictionary of Contest Entries for the User key'd by their contestId
    public Dictionary<string, ContestEntry> ContestEntries;

    public User(string displayName, string email, System.Action callbackOnComplete)
    {
        _displayName = displayName;
        _emailAddress = email;

        callbackOnLoadComplete = callbackOnComplete;
        Load();
    }

    private void Load()
    {
        UpdateLocalPlayerContests();
    }

    public void AddFantasyContestLineup(string contestId, string slotPlayer1, string slotPlayer2, string slotPlayer3, string slotPlayer4, string slotTeam1)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("addPlayerContestLineup").SetEventAttribute("contestId", contestId)
            .SetEventAttribute("slotPlayer1", slotPlayer1).SetEventAttribute("slotPlayer2", slotPlayer2)
            .SetEventAttribute("slotPlayer3", slotPlayer3).SetEventAttribute("slotPlayer4", slotPlayer4)
            .SetEventAttribute("slotTeam1", slotTeam1)
            .Send((response =>
            {
                if (response.HasErrors)
                {
                    Logging.Log(response.Errors);
                }
                else
                {
                    Logging.Log(response.JSONString);
                    UpdateLocalPlayerContests();

                }
            }));
    }

    public void UpdateLocalPlayerContests()
    {
        ContestEntries = new Dictionary<string, ContestEntry>();

        // Load the user's fantasy contest data from the playerData collection on mongoDB
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("getPlayerContestLineups").Send((response) =>
        {
            
            if (response.HasErrors)
            {
                Logging.LogError("getPlayerContestLineups returned errors: " + response.Errors.JSON);
            }
            else
            {
                Logging.Log(response.JSONString);

                if (response.BaseData != null)
                {
                    GameSparks.Core.GSData data = response.ScriptData;
                    if (data == null)
                    {
                        Logging.LogError("ScriptData is NULL");
                        return;
                    }

                    List<GameSparks.Core.GSData> dataList = data.GetGSDataList("contestLineups");
                    if (dataList == null)
                    {
                        Logging.LogError("Couldn't get contestLineups GSData");
                        return;
                    }

                    // Iterate over the response of contestLineups (could be empty!)
                    foreach (var obj in dataList)
                    {
                        ContestEntry entry = new ContestEntry();
                        entry.ContestID = obj.GetString("contestId");
                        
                        // Convert the player strings into fantasy player objects for the ContestEntry
                        string player1 = obj.GetString("slotPlayer1");
                        if(!string.IsNullOrEmpty(player1))
                        {
                            entry.SlotPlayer1 = FantasyManager.Instance.GetPlayerFromName(player1);
                        }

                        string player2 = obj.GetString("slotPlayer2");
                        if (!string.IsNullOrEmpty(player2))
                        {
                            entry.SlotPlayer2 = FantasyManager.Instance.GetPlayerFromName(player2);
                        }

                        string player3 = obj.GetString("slotPlayer3");
                        if (!string.IsNullOrEmpty(player3))
                        {
                            entry.SlotPlayer3 = FantasyManager.Instance.GetPlayerFromName(player3);
                        }

                        string player4 = obj.GetString("slotPlayer4");
                        if (!string.IsNullOrEmpty(player4))
                        {
                            entry.SlotPlayer4 = FantasyManager.Instance.GetPlayerFromName(player4);
                        }

                        entry.SlotTeam1 = obj.GetString("slotTeam1");

                        if(ContestEntries.ContainsKey(entry.ContestID))
                        {
                            ContestEntries[entry.ContestID] = entry;
                        }
                        else
                        {
                            ContestEntries.Add(entry.ContestID, entry);
                        }
                    }

                    if (callbackOnLoadComplete != null)
                    {
                        callbackOnLoadComplete();
                    }
                }
                else
                {
                    Logging.LogError("response Base Data = null");
                }
            }


        });

    }
}
