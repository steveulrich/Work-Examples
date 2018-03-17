using UnityEngine;

public class FantasyPlayerSlot : MonoBehaviour
{
    public int SlotNumber;
    public UnityEngine.UI.Image PlayerPicture;
    public UnityEngine.UI.Text PlayerSalary;
    public UnityEngine.UI.Text PlayerName;
    public UnityEngine.UI.Text PlayerTeam;
    public UnityEngine.UI.Text PlayerFPPGs;

    public GameObject EditButton;


    public void ConfigureSlot(FantasyPlayer player)
    {
        ConfigureSlot(player.Name, player.Team, player.Points, player.Salary, player.Picture);
    }
    public void ConfigureSlot(string playerName, string playerTeam, string playerFPPG, string playerSalary, Sprite playerPicture)
    {
        PlayerName.text = playerName;
        PlayerTeam.text = playerTeam;
        PlayerFPPGs.text = playerFPPG;
        PlayerSalary.text = playerSalary;
        PlayerPicture.sprite = playerPicture;
    }
}
