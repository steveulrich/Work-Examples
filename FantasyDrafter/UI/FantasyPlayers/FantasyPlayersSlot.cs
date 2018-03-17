using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FantasyPlayersSlot : MonoBehaviour {

    public UnityEngine.UI.Image PlayerImage;
    public UnityEngine.UI.Text PlayerSalary;
    public UnityEngine.UI.Text PlayerName;
    public UnityEngine.UI.Text PlayerTeam;
    public UnityEngine.UI.Text PlayerFPPGs;

    public UnityEngine.UI.Button RemoveButton;
    public UnityEngine.UI.Button AddButton;

    public void Configure(FantasyPlayer player)
    {
        Configure(player.Name, player.Salary, player.Team, player.Points, player.Picture);
    }

    public void Configure(string name, string salary, string team, string fantasyPointsPerGame, Sprite playerImage = null)
    {
        PlayerName.text = name;
        PlayerSalary.text = salary;
        PlayerTeam.text = team;
        PlayerFPPGs.text = fantasyPointsPerGame;
        PlayerImage.sprite = playerImage;
        PlayerImage.preserveAspect = true;

        PlayerImage.enabled = playerImage != null;
    }

}
