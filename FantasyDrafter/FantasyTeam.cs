using System.Collections.Generic;

public class FantasyTeam
{
    public string TeamName;
    public int Salary;
    public string FPPG;
    public UnityEngine.Sprite Logo;

    public List<FantasyPlayer> Players;
}

public class FantasyTeamComparer : IComparer<FantasyTeam>
{
    int IComparer<FantasyTeam>.Compare(FantasyTeam x, FantasyTeam y)
    {
        if (x.Salary == y.Salary)
        {
            return 0;
        }
        else if (x.Salary > y.Salary)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
