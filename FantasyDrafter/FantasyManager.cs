using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class FantasyManager : Singleton<FantasyManager>
{
    public System.DateTime NextMatchDate;
    public System.Int64 CurrentWeek;

    public List<FantasyPlayer> FantasyPlayersList = new List<FantasyPlayer>();
    public Dictionary<string, FantasyTeam> FantasyTeams = new Dictionary<string, FantasyTeam>();

    // Unused
    public Dictionary<string, FantasyTeam> StandingsDictionary = new Dictionary<string, FantasyTeam>();

    IEnumerator Start()
    {
        yield return Backend.Utility.WaitForFrame;

        System.Action getFantasyTeams = delegate
        {
            FantasyUtility.GetFantasyTeamList(null);
        };
        // Get the FantasyTeams from the GameSparks cloud
        LoadFantasyPlayersFromGameSparks(getFantasyTeams);
    }

    private void LoadFantasyPlayersFromGameSparks(System.Action cbOnComplete)
    {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("getPlayers")
            .SetEventAttribute("team", "")
            .Send((response) =>
            {
                if(!response.HasErrors)
                {
                    Logging.Log("Received Fantasy Players full list from GameSparks: " + response.JSONString.ToString());

                    if(response.BaseData != null)
                    {
                        GameSparks.Core.GSData data = response.ScriptData;
                        if(data == null)
                        {
                            Logging.LogError("ScriptData is NULL");
                            return;
                        }

                        List<GameSparks.Core.GSData> dataList = data.GetGSDataList("fantasyPlayers");   
                        if(dataList == null)
                        {
                            Logging.LogError("Couldn't get FantasyPlayers GSData");
                            return;
                        }

                        foreach(var obj in dataList)
                        {
                            FantasyPlayer player = new FantasyPlayer();

                            player.Name = obj.GetString("name");
                            player.Team = obj.GetString("team");
                            player.Salary = obj.GetString("salary");
                            player.Points = obj.GetString("fppg");

                            System.Action<Texture2D> imageReceivedCB = delegate (Texture2D image)
                            {
                                Sprite playerImage = null;
                                if (image != null)
                                {
                                    playerImage = Sprite.Create(image, new Rect(0.0f, 0.0f, image.width, image.height), new Vector2(0.5f, 0.5f));
                                    player.Picture = playerImage;
                                }
                            };

                            FantasyManager.Instance.DownloadAnImage(obj.GetString("portraitShortCode"), imageReceivedCB);
                            
                            FantasyPlayersList.Add(player);
                        }

                        EventManager.instance.TriggerEvent(new EventFantasyPlayersListUpdated(System.DateTime.Now));
                    }
                    else
                    {
                        Logging.LogError("response Base Data = null");
                    }

                    if(cbOnComplete != null)
                    {
                        cbOnComplete.Invoke();
                    }
                }
            });
    }

    public void LoadStandingsJSON(string json)
    {
        if (json.Length > 0)
        {
            try
            {
                // try to deserialize the json from the file
                Dictionary<string, object> teamDict = MiniJSON.Json.Deserialize(json) as Dictionary<string, object>;
                if (teamDict != null)
                {
                    // jsonDict is a Dictionary<string, object> where object is a Dictionary<string, object> at this point
                    foreach (var team in teamDict)
                    {
                        Logging.Log(team.Key);

                        StandingsDictionary[team.Key] = new FantasyTeam();
                        StandingsDictionary[team.Key].TeamName = team.Key;

                        // categoryKVP is a KeyValuePair, where Key is the category, and value is a Dictionary<string, object>
                        Dictionary<string, object> teamInfoDict = team.Value as Dictionary<string, object>;

                        foreach (var info in teamInfoDict)
                        {
                            Logging.Log("    " + info.Key);

                            switch (info.Key)
                            {
                                case "Rank":
                                    // Description Section is just a string
                                    string rank = info.Value as string;
                                    StandingsDictionary[team.Key].Salary = int.Parse(rank);
                                    break;
                                case "Record":
                                    // Color section is a hex string
                                    StandingsDictionary[team.Key].FPPG = info.Value as string;
                                    break;
                            }


                        }
                    }

                }
                else
                {
                    Logging.LogWarning("Unable to deserialize json");
                }
            }
            catch (System.Exception e) // if someone makes a mistake in the dynamic properties, then we should try to go on without them
            {
                Logging.LogException(e);
                Logging.LogError("Error while parsing dynamic propertied! " + e.Message);
            }
        }
        else
        {
            Logging.LogError("Unable to load ProofPoint Info from empty json!");
        }
    }

    private void GetNextMatchDate()
    {
        Backend.DynamicProgramProperties.TryGetValue("CurrentWeek", out CurrentWeek);

        string startDateString = string.Empty;
        Backend.DynamicProgramProperties.TryGetValue("FirstWeek", out startDateString);

        System.DateTime startDate = Backend.Utility.ConvertTimestampToDatetime(startDateString);

        Logging.Log("StartDate: " + startDate.ToString());

        NextMatchDate = startDate.AddDays(7 * CurrentWeek);

    }

    public List<FantasyPlayer> GetPlayersOnTeam(string team)
    {
        List<FantasyPlayer> playersOnTeam = FantasyPlayersList.FindAll(delegate (FantasyPlayer player)
        {
            return player.Team == team;
        });

        return playersOnTeam;

    }

    public FantasyPlayer GetPlayerFromName(string name)
    {
        FantasyPlayer player = FantasyPlayersList.Find(fPlayer => fPlayer.Name.Equals(name));
        if(player != null)
        {
            return player;
        }

        return null;
    }

    public List<FantasyTeam> GetFantasyTeamListByNames(string[] names)
    {
        List<FantasyTeam> teams = new List<FantasyTeam>();

        foreach(var name in names)
        {
            if (FantasyTeams.ContainsKey(name))
            {
                teams.Add(FantasyTeams[name]);
            }
        }

        return teams;
    }

    //When we want to download our uploaded image
    public void DownloadAnImage(string fileId, System.Action<Texture2D> imageReceivedCallback)
    {
        //Get the url associated with the uploadId
        new GameSparks.Api.Requests.GetDownloadableRequest().SetShortCode(fileId).Send((response) =>
        {
            //pass the url to our coroutine that will accept the data
            StartCoroutine(DownloadImage(response.Url, imageReceivedCallback));
        });
    }


    public IEnumerator DownloadImage(string downloadUrl, System.Action<Texture2D> imageCallback)
    {
        Texture2D downloadedImage = new Texture2D(200, 200, TextureFormat.ARGB32, false);

        if(!string.IsNullOrEmpty(downloadUrl))
        {
            var www = new WWW(downloadUrl);

            yield return www;

            www.LoadImageIntoTexture(downloadedImage);
        }

        if (imageCallback != null)
        {
            imageCallback(downloadedImage);
            imageCallback = null;
        }
    }

}
