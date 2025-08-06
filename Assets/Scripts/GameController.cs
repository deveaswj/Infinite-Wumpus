using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    public Text gameLogText;
    public Button[] exitButtons; // Assign 3 buttons in the Inspector
    public Button stairsButton;
    public Text healthText;
    public Text treasureText;

    private Player player;
    private Dungeon dungeon;
    private Room currentRoom;

    void Start()
    {
        player = new Player();
        dungeon = new Dungeon();
        currentRoom = dungeon.GetStartingRoom();
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

    public void OnStairsButton()
    {
        if (currentRoom.HasStairsDown)
        {
            dungeon.GoDownstairs(currentRoom);
            currentRoom = dungeon.GetCurrentRoom();
            player.AssignDonutIfEligible(dungeon.CurrentLevel);
            Log("You descend the stairs...");
        }
        else if (currentRoom.HasStairsUp)
        {
            dungeon.GoUpstairs(currentRoom);
            currentRoom = dungeon.GetCurrentRoom();
            Log("You climb the stairs...");
        }
        else
        {
            Log("There are no stairs here.");
            return;
        }

        HandleRoomEntry();
    }

    void HandleRoomEntry()
    {
        if (currentRoom.HasTreasure && !currentRoom.Collected)
        {
            player.CollectTreasure();
            currentRoom.Collected = true;
            Log("You found a treasure!");
        }

        if (currentRoom.IsPit)
        {
            Log("You stumble into a pit and fall!");
            player.TakeDamage();
            dungeon.FallToNextLevel();
            currentRoom = dungeon.GetCurrentRoom();
        }

        if (currentRoom.HasWumpus)
        {
            if (player.HasDonut)
            {
                Log("The Wumpus smells your donut, steals it, and flees!");
                player.UseDonut();
                currentRoom.HasWumpus = false;
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
            exitButtons[i].GetComponentInChildren<Text>().text = $"Go to Room {currentRoom.Exits[i].ID}";
        }

        stairsButton.gameObject.SetActive(currentRoom.HasStairsDown || currentRoom.HasStairsUp);
        stairsButton.GetComponentInChildren<Text>().text = currentRoom.HasStairsDown ? "Descend" : "Ascend";

        healthText.text = $"HP: {player.Health}";
        treasureText.text = $"Treasure: {player.TreasureCount}";

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
        stairsButton.interactable = false;
    }
}
