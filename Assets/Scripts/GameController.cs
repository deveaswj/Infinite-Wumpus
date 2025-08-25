using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int dungeonSeed = 0;

    public RollingTextHistory narrator;

    public Button[] exitButtons; // Assign 3 buttons in the Inspector
    public Button stairsUpButton;
    public Button stairsDownButton;

    public Button restButton;

    public GameObject playButtons;

    public TMP_Text healthText;
    public TMP_Text scoreText;

    private Player player;
    private Dungeon dungeon;
    private GameRules gameRules;

    void Start()
    {
        StartGame();
    }

    // --- Game flow --------------------------------------

    void StartGame()
    {
        SetPlayMode();

        dungeon = new Dungeon(dungeonSeed);
        player = new Player(dungeon);
        gameRules = new GameRules(dungeon, player, narrator);

        player.EnterDungeon();  // calls MoveTo(0,0)

        ClearLog();
        Log("You awaken in a dark room...");

        gameRules.OnPlayerEnterRoom(player);  // there might be pits or bats nearby
        UpdateUI();
    }

    private void MovePlayerTo(Room room) => MovePlayerTo(room, null);

    private void MovePlayerTo(Room room, string transitionText)
    {
        ClearLog();
        if (transitionText != null) Log(transitionText);
        player.MoveTo(room.Level, room.ID);
        gameRules.OnPlayerEnterRoom(player);
        UpdateUI();
    }

    public void OnPlayButton()
    {
        StartGame();
    }

    public void OnExitButton(int index)
    {
        Room playerRoom = dungeon.GetRoom(player);

        if (index >= playerRoom.Exits.Count)
        {
            Log("That way is blocked.");
            UpdateUI();
        }
        else
        {
            Room nextRoom = playerRoom.Exits[index];
            // , $"You head to level {nextRoom.Level}, room {nextRoom.ID}"
            MovePlayerTo(nextRoom);
        }
    }

    public void OnUpstairsButton()
    {
        Room playerRoom = dungeon.GetRoom(player);

        if (playerRoom.HasStairsUp)
        {
            player.Ascend();
            Room nextRoom = dungeon.GetRoom(player);
            MovePlayerTo(nextRoom, "You ascend the stairs...");
        }
        else
        {
            Log("There are no stairs leading up from here.");
            UpdateUI();
            return;
        }
    }

    public void OnDownstairsButton()
    {
        Room playerRoom = dungeon.GetRoom(player);

        if (playerRoom.HasStairsDown)
        {
            player.Descend();
            Room nextRoom = dungeon.GetRoom(player);
            MovePlayerTo(nextRoom, "You descend the stairs...");
        }
        else
        {
            Log("There are no stairs leading down from here.");
            UpdateUI();
            return;
        }
    }

    public void OnRestButton()
    {
        Log("You rest for a while... but the bats don't...");

        // Player heals +1, but all live hazards in the level (bats, Wumpus) move
        player.Heal(1);

        DungeonLevel playerLevel = dungeon.GetLevel(player);
        playerLevel.MoveAllBats();
        if (playerLevel.WumpusState == WumpusState.Idle)
        {
            Log("... and neither does the Wumpus.");
            playerLevel.MoveWumpus();
        }

        gameRules.OnPlayerEnterRoom(player);
        UpdateUI();
    }


    // --- UI ---------------------------------------------

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

        healthText.text = $"HP: {player.Health}";
        scoreText.text = $"Score: {player.Score}";

        if (player.IsDead())
        {
            Log("You have died... Game over.");
            SetGameOverMode();
        }
        else
        {
            bool isInjured = player.IsInjured();
            restButton.gameObject.SetActive(isInjured);
        }
    }

    void SetPlayMode()
    {
        playButtons.SetActive(false);
        foreach (var btn in exitButtons)
            btn.interactable = true;
        stairsUpButton.interactable = true;
        stairsDownButton.interactable = true;
    }

    void SetGameOverMode()
    {
        playButtons.SetActive(true);
        foreach (var btn in exitButtons)
            btn.interactable = false;
        stairsUpButton.interactable = false;
        stairsDownButton.interactable = false;
    }


    // --- Logging ----------------------------------------

    void Log(string message)
    {
        narrator.Say(message);
        Debug.Log("Say: " + message);
    }

    void ClearLog()
    {
        narrator.ClearHistory();
        Debug.Log("Log cleared");
    }


}
