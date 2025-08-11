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
    private Room currentRoom;

    void Start()
    {
        player = new Player();
        dungeon = new Dungeon();

        player.EnterDungeon(dungeon);
        currentRoom = dungeon.GetRoom(0, 0);

        player.MoveTo(currentRoom.ID, 0);
        UpdateUI();
        Log("You awaken in a dark room...");
    }

    public void OnExitButton(int index)
    {
        if (index >= currentRoom.Exits.Count)
        {
            Log("That way is blocked.");
            return;
        }

        Room nextRoom = currentRoom.Exits[index];
        currentRoom = nextRoom;
        HandleRoomEntry();
    }


    public void OnUpstairsButton()
    {
        if (currentRoom.HasStairsUp)
        {
            dungeon.Ascend(currentRoom);
            currentRoom = dungeon.GetCurrentRoom();
            Log("You climb the stairs...");
            HandleRoomEntry();
        }
        else
        {
            Log("There are no stairs here.");
            return;
        }
    }

    public void OnDownstairsButton()
    {
        if (currentRoom.HasStairsDown)
        {
            dungeon.Descend(currentRoom);
            currentRoom = dungeon.GetCurrentRoom();
            Log("You descend the stairs...");
            HandleRoomEntry();
        }
        else
        {
            Log("There are no stairs here.");
            return;
        }
    }

    void HandleRoomEntry()
    {
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
        for (int i = 0; i < exitButtons.Length; i++)
        {
            exitButtons[i].gameObject.SetActive(i < currentRoom.Exits.Count);
            exitButtons[i].GetComponentInChildren<TMP_Text>().text = $"Room {currentRoom.Exits[i].ID}";
        }

        stairsUpButton.gameObject.SetActive(currentRoom.HasStairsUp);
        stairsDownButton.gameObject.SetActive(currentRoom.HasStairsDown);

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
