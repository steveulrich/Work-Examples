using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Requests;

public class FantasyUtility
{
    /// <summary>
    /// Get a list of Fantasy Players from game sparks
    /// </summary>
    /// <returns>A list containing Fantasy Player objects for each player</returns>
    public static List<FantasyPlayer> GetFantasyPlayerList(System.Action callbackOnComplete)
    {
        List<FantasyPlayer> players = new List<FantasyPlayer>();

        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("getFantasyTeams")
            .SetEventAttribute("team", "")
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Logging.Log("Received FantasyPlayers from GameSparks: " + response.JSONString.ToString());

                    if (response.BaseData != null)
                    {
                        GameSparks.Core.GSData data = response.ScriptData;
                        if (data != null)
                        {
                            List<GameSparks.Core.GSData> dataList = data.GetGSDataList("fantasyPlayers");
                            if (dataList == null)
                            {
                                foreach (var obj in dataList)
                                {
                                    // Populate a new fantasy team based on the info pulled from the dataList
                                    string name = obj.GetString("name");
                                    FantasyPlayer player = new FantasyPlayer(null, null, null, null, null);

                                    // @TODO: Continue populating team info

                                    players.Add(player);
                                }
                            }
                            else
                            {
                                Logging.LogError("Couldn't get FantasyPlayers GSData");
                            }

                        }
                        else
                        {
                            Logging.LogError("FantasyPlayers ScriptData is NULL");
                        }
                    }
                    else
                    {
                        Logging.LogError("FantasyPlayers response Base Data = null");
                    }
                }
                else
                {
                    Logging.Log("Error retrieving FantasyPlayers from GameSparks: " + response.Errors.ToString());
                }

                if (callbackOnComplete != null)
                {
                    callbackOnComplete();
                }

            });

        return players;

    }

    /// <summary>
    /// Get a list of FantasyTeams from teh FantasyManager's standings dictionary
    /// </summary>
    /// <returns>A list containing FantasyTeam objects for each team</returns>
    public static void GetFantasyTeamList(System.Action callbackOnComplete)
    {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("getFantasyTeams")
            .SetEventAttribute("team", "")
            .Send((response) =>
            {
                if(!response.HasErrors)
                {
                    Logging.Log("Received FantasyTeams from GameSparks: " + response.JSONString.ToString());

                    if (response.BaseData != null)
                    {
                        GameSparks.Core.GSData data = response.ScriptData;
                        if (data != null)
                        {
                            List<GameSparks.Core.GSData> dataList = data.GetGSDataList("fantasyTeams");
                            if (dataList != null)
                            {
                                foreach (var obj in dataList)
                                {
                                    // Populate a new fantasy team based on the info pulled from the dataList
                                    FantasyTeam team = new FantasyTeam();
                                    team.TeamName = obj.GetString("teamName");

                                    // Continue populating team info
                                    team.Players = FantasyManager.Instance.GetPlayersOnTeam(team.TeamName);
                                    team.Salary = int.Parse(obj.GetString("salary"));
                                    team.FPPG = obj.GetString("fppg");

                                    // Grab the corresponding logo image for this team
                                    System.Action<Texture2D> imageReceived = delegate (Texture2D image)
                                    {
                                        team.Logo = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    };
                                    FantasyManager.Instance.DownloadAnImage(obj.GetString("logoShortCode"), imageReceived);

                                    // Add the fantasy team to some persistent data structure
                                    if (!FantasyManager.Instance.FantasyTeams.ContainsKey(team.TeamName))
                                    {
                                        FantasyManager.Instance.FantasyTeams.Add(team.TeamName, team);
                                    }
                                    else
                                    {
                                        FantasyManager.Instance.FantasyTeams[team.TeamName] = team;
                                    }
                                }
                            }
                            else
                            {
                                Logging.LogError("Couldn't get FantasyTeams GSData");
                            }

                        }
                        else
                        {
                            Logging.LogError("FantasyTeams ScriptData is NULL");
                        }
                    }
                    else
                    {
                        Logging.LogError("FantasyTeams response Base Data = null");
                    }
                }
                else
                {
                    Logging.Log("Error retrieving FantasyTeams from GameSparks: " + response.Errors.ToString());
                }

                if (callbackOnComplete != null)
                {
                    callbackOnComplete();
                }

            });
    }


    public static void GetContestInfoFromGameSparks(System.Action callbackOnComplete)
    {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("getContests")
            .SetEventAttribute("contestId", "")
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Logging.Log("Received ContestInfos from GameSparks: " + response.JSONString.ToString());

                    if (response.BaseData != null)
                    {
                        GameSparks.Core.GSData data = response.ScriptData;
                        if (data != null)
                        {
                            List<GameSparks.Core.GSData> dataList = data.GetGSDataList("fantasyContests");
                            if (dataList != null)
                            {
                                int i = 0;
                                foreach (var obj in dataList)
                                {
                                    ContestInfo info = new ContestInfo();
                                    info.contestID = obj.GetString("contestId");
                                    info.contestTitle = obj.GetString("name");
                                    info.contestStartDate = obj.GetString("startdate") + " " + obj.GetString("starttime");

                                    // Grab the corresponding logo image for this contest
                                    System.Action<Texture2D> imageReceived = delegate (Texture2D image)
                                    {
                                        i++;
                                        info.contestLogo = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f), 100.0f);
                                        ContestsView.CurrentContests[info.contestID] = info;

                                        if(i == dataList.Count)
                                        {
                                            if (callbackOnComplete != null)
                                            {
                                                callbackOnComplete();
                                            }

                                        }
                                    };

                                    FantasyManager.Instance.DownloadAnImage(obj.GetString("logoShortCode"), imageReceived);
                                    // Populate the contest with teams list
                                    string teamsCSV = obj.GetString("teamNames");
                                    if(teamsCSV != null)
                                    {
                                        Logging.Log("Teams: " + teamsCSV);
                                        string[] teamsSplit = teamsCSV.Split(',');
                                        if (teamsSplit.Length > 0)
                                        {
                                            info.Teams = FantasyManager.Instance.GetFantasyTeamListByNames(teamsSplit);
                                        }
                                    }
                                    else
                                    {
                                        Logging.LogError("No teams found for contest: " + info.contestID);
                                    }
                                }
                            }
                            else
                            {
                                Logging.LogError("Couldn't get FantasyContests GSData");
                            }

                        }
                        else
                        {
                            Logging.LogError("FantasyContests ScriptData is NULL");
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
