
[System.Serializable]
public class FantasyPlayer
{
    public string Name;
    public string Team;
    public string Salary;
    public string Points;
    public UnityEngine.Sprite Picture;
    public int OptionSlot;
    public int ContestSlot;

    public void Configure(FantasyPlayer player)
    {
        Configure(player.Name, player.Team, player.Salary, player.Points, player.Picture);
    }

    public void Configure(string name, string team, string rating, string points, UnityEngine.Sprite picture)
    {
        Name = name;
        Team = team;
        Salary = rating;
        Points = points;
        Picture = picture;
    }

    public FantasyPlayer(string name, string team, string rating, string points, UnityEngine.Sprite picture)
    {
        Name = name;
        Team = team;
        Salary = rating;
        Points = points;
        Picture = picture;
    }
}
