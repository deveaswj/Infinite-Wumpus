using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TMP_Text gameLogText;
    public Button[] exitButtons; // Assign 3 buttons in the Inspector
    public Button stairsUpButton;
    public Button stairsDownButton;

    //public Text healthText;
    //public Text treasureText;

    private Player player;
    private Dungeon dungeon;
//    private Room currentRoom;

    void Start()
    {
        dungeon = new Dungeon();
        player = new Player(dungeon);

        player.EnterDungeon();  // calls MoveTo(0,0)
        UpdateUI();
        Log("You awaken in a dark room...");
    }

    public void OnExitButton(int index)
    {
        Room playerRoom = dungeon.GetRoom(player);

        if (index >= playerRoom.Exits.Count)
        {
            Log("That way is blocked.");
            return;
        }

        Room nextRoom = playerRoom.Exits[index];
        player.MoveTo(nextRoom.Level, nextRoom.ID);

        HandleRoomEntry();
    }


    public void OnUpstairsButton()
    {
        Room playerRoom = dungeon.GetRoom(player);

        if (playerRoom.HasStairsUp)
        {
            player.Ascend();
            Log("You climb the stairs...");

            HandleRoomEntry();
        }
        else
        {
            Log("There are no stairs leading up from here.");
            return;
        }
    }

    public void OnDownstairsButton()
    {
        Room playerRoom = dungeon.GetRoom(player);

        if (playerRoom.HasStairsDown)
        {
            player.Descend();
            Log("You descend the stairs...");

            HandleRoomEntry();
        }
        else
        {
            Log("There are no stairs leading down from here.");
            return;
        }
    }

    void HandleRoomEntry()
    {
        Room playerRoom = dungeon.GetRoom(player);
        Room currentRoom = playerRoom;

        if (currentRoom.HasTreasure)
        {
            player.CollectTreasure();
            Log("You found a treasure!");
        }

        if (currentRoom.HasPit)
        {
            Log("You stumble into a pit and fall!");
            player.TakeDamage();
            dungeon.Descend(currentRoom);
            currentRoom = dungeon.GetCurrentRoom();
        }

        if (currentRoom.HasWumpus)
        {
            if (player.HasDonut)
            {
                Log("The Wumpus smells your donut, steals it, and flees!");
                player.UseDonut();
                //
                // wumpus runs away and is no longer in the room
                //
                // currentRoom.HasWumpus = false;
            }
            else
            {
                Log("The Wumpus attacks! You take damage.");
                player.TakeDamage();
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        Room playerRoom = dungeon.GetRoom(player);

        for (int i = 0; i < exitButtons.Length; i++)
        {
            exitButtons[i].gameObject.SetActive(i < playerRoom.Exits.Count);
            exitButtons[i].GetComponentInChildren<TMP_Text>().text = $"Room {playerRoom.Exits[i].ID}";
        }

        stairsUpButton.gameObject.SetActive(playerRoom.HasStairsUp);
        stairsDownButton.gameObject.SetActive(playerRoom.HasStairsDown);

        // healthText.text = $"HP: {player.Health}";
        // treasureText.text = $"Treasure: {player.TreasureCount}";

        if (player.IsDead())
        {
            Log("You have died... Game over.");
            DisableInput();
        }
    }

    void Log(string message)
    {
        gameLogText.text = message;
    }

    void DisableInput()
    {
        foreach (var btn in exitButtons)
            btn.interactable = false;
        stairsUpButton.interactable = false;
        stairsDownButton.interactable = false;
    }
}
