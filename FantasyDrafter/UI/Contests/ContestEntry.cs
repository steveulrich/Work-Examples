
[System.Serializable]
public class ContestEntry
{
    private readonly string EMPTY = "Empty";

    public string ContestID;
    public FantasyPlayer SlotPlayer1 = null;
    public FantasyPlayer SlotPlayer2 = null;
    public FantasyPlayer SlotPlayer3 = null;
    public FantasyPlayer SlotPlayer4 = null;
    public string SlotTeam1;
    public int CurrentSalary;

    public int AddPlayer(FantasyPlayer player)
    {
        int slotIndex = -1;

        if (SlotPlayer1.Name == EMPTY)
        {
            SlotPlayer1.Configure(player);
            slotIndex = 0;
        }
        else if (SlotPlayer2.Name == EMPTY)
        {
            SlotPlayer2.Configure(player);
            slotIndex = 1;
        }
        else if (SlotPlayer3.Name == EMPTY)
        {
            SlotPlayer3.Configure(player);
            slotIndex = 2;
        }
        else if (SlotPlayer4.Name == EMPTY)
        {
            SlotPlayer4.Configure(player);
            slotIndex = 3;
        }
        else
        {
            Logging.LogError("Unable to add player to slot");
        }

        return slotIndex;

    }

    public int RemovePlayer(FantasyPlayer player)
    {
        int slotIndex = -1;

        if (SlotPlayer1.Name == player.Name)
        {
            SlotPlayer1.Configure(EMPTY, string.Empty, string.Empty, string.Empty, null);
            slotIndex = 0;
        }
        else if (SlotPlayer2.Name == player.Name)
        {
            SlotPlayer2.Configure(EMPTY, string.Empty, string.Empty, string.Empty, null);
            slotIndex = 1;
        }
        else if (SlotPlayer3.Name == player.Name)
        {
            SlotPlayer3.Configure(EMPTY, string.Empty, string.Empty, string.Empty, null);
            slotIndex = 2;
        }
        else if (SlotPlayer4.Name == player.Name)
        {
            SlotPlayer4.Configure(EMPTY, string.Empty, string.Empty, string.Empty, null);
            slotIndex = 3;
        }
        else
        {
            Logging.LogError("Unable to remove player from slot");
        }

        return slotIndex;
    }

    public bool IsReady()
    {
        return SlotPlayer1.Name != EMPTY && SlotPlayer2.Name != EMPTY && SlotPlayer3.Name != EMPTY && SlotPlayer4.Name != EMPTY && SlotTeam1 != EMPTY;
    }

}