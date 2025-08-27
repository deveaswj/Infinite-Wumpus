// Responsible for enforcing game rules
using System.Linq;
using UnityEngine;

public class GameRules
{
    // determines whether to show hazard locations in the text
    public bool cheatMode = false;

    private Dungeon dungeon;
    private Player player;
    private RollingTextHistory narrator;

    private StringQueue logQueue = new StringQueue(10);

    private bool showInjuryMessage = false;

    public GameRules(Dungeon dungeon, Player player, RollingTextHistory narrator)
    {
        this.dungeon = dungeon;
        this.player = player;
        this.narrator = narrator;
        logQueue.Clear();
    }

    public void OnPlayerEnterRoom(Player player)
    {
        TakePlayerTurn(player);
    }

    public void TakePlayerTurn(Player player)
    {
        var room = dungeon.GetRoom(player);
        QueueRoomDescription(room);
        narrator.Say(logQueue.GetQueue());
        logQueue.Clear();

        if (HandlePit(player, room)) return;

        //  What shall we do if there are bats AND a Wumpus in the room?
        //  For now, just prevent bats and Wumpus from coexisting in a room.
        HandleWumpus(player, room);
        if (HandleBats(player, room)) return;

        HandleInjuryAndAutoDonut(player);

        HandlePickups(player, room);
    }

    private bool HandlePit(Player player, Room room)
    {
        if (!room.HasPit) return false;

        string fallHow = "You fall headlong into a pit.";
        // get a random number to choose from a list of various ways to fall into a pit
        // define the list
        string[] fallHowList = new string[] {
            "Arms flailing, you fall into a pit.",
            "You fall through a pit and land one level down.",
            "You fall coyote-style into a pit.",
            "You trip into a dank, breezy pit.",
            "As you fall, you briefly reconsider your life choices.",
            "You fall headlong into a pit."
        };
        int fallHowIndex = Random.Range(0, fallHowList.Length);
        fallHow = fallHowList[fallHowIndex];
 
        narrator.Say(fallHow);
        player.Fall();
        if (player.IsDead()) return true;

        showInjuryMessage = true;
        narrator.Say($"You find yourself in level {player.CurrentLevel}, room {player.CurrentRoomID}.");
        OnPlayerEnterRoom(player); // recursion okay here, but safe as only one fall occurs
        return true;
    }

    private void HandleWumpus(Player player, Room room)
    {
        DungeonLevel playerLevel = dungeon.GetLevel(player);

        if (playerLevel.WumpusState == WumpusState.None) return;

        // if the Wumpus is asleep (anywhere), update its alarm (wake up if needed)
        if (playerLevel.WumpusState == WumpusState.Asleep)
        {
            playerLevel.UpdateWumpusAlarm();
        }

        // if the Wumpus isn't here, return
        if (!room.HasWumpus) return;

        // if the Wumpus is here but asleep, tell the player and return
        if (playerLevel.WumpusState == WumpusState.Asleep)
        {
            narrator.Say("The Wumpus here is curled up and snoring loudly.");
            return;
        }

        bool moveWumpus = true;

        if (player.HasDonut)
        {
            narrator.Say("The Wumpus grabs your donut and runs away!");
            player.LoseDonut();
            // put the Wumpus to sleep before moving it
            playerLevel.SleepWumpus(5);
        }
        else
        {
            narrator.Say("The Wumpus shrieks hungrily and attacks you!");
            player.TakeDamage(1);
            if (!player.IsDead())
            {
                showInjuryMessage = true;

                // roll to see if we move the Wumpus
                int roll = Random.Range(1, 7); // D6
                moveWumpus = roll > 3;
                if (moveWumpus)
                {
                    narrator.Say("By the time you recover, the Wumpus is gone.");
                }
                else
                {
                    narrator.Say("It looms over you, growling and drooling and stinking.");
                }
            }
        }

        if (moveWumpus) playerLevel.MoveWumpus();
    }

    private bool HandleBats(Player player, Room room)
    {
        if (!room.HasBats) return false;
        narrator.Say("You are swarmed by bats, and stumble around...");
        dungeon.GetLevel(player).MoveBats(room);
        var safeRoom = dungeon.GetLevel(player).GetRandomSafeRoom();
        player.MoveTo(safeRoom);
        narrator.Say($"You recover in level {player.CurrentLevel}, room {player.CurrentRoomID}.");
        OnPlayerEnterRoom(player);
        return true;
    }

    private void HandleInjuryAndAutoDonut(Player player)
    {
        if (!player.IsInjured()) return;

        if (showInjuryMessage)
        {
            narrator.Say("You are injured!");
            showInjuryMessage = false;
        }

        if (player.HasDonut)
        {
            player.UseDonut();
            narrator.Say($"You eat your donut and feel{(player.IsInjured() ? " a little" : " all")} better.");
        }
    }

    private void HandlePickups(Player player, Room room)
    {
        if (room.HasTreasure)
        {
            var treasure = room.ClaimTreasure();
            narrator.Say($"You have found {treasure.Description}!");
            player.AddScore(treasure.Value);
        }

        if (room.HasDonut)
        {
            if (player.HasDonut)
                narrator.Say("There is a donut here, but you already have one.");
            else
            {
                narrator.Say("You have found a donut! This may come in handy.");
                player.GainDonut();
                room.SetDonut(false);
            }
        }
    }

    public void OnPlayerRest()
    {
        // Player heals +1, but all live hazards in the level (bats, Wumpus) move
        player.Heal(1);

        narrator.Say("You rest for a while... but the bats don't...");

        DungeonLevel playerLevel = dungeon.GetLevel(player);
        playerLevel.MoveAllBats();

        // move the Wumpus if it's idle (ignore if it's asleep)
        if (playerLevel.WumpusState == WumpusState.Idle)
        {
            narrator.Say("... and neither does the Wumpus.");
            playerLevel.MoveWumpus();
        }
    }

    void QueueRoomDescription(Room room)
    {
        // room location debug message
        logQueue.Enqueue("-- You are in room " + room.ID + " (level " + room.Level + ") --");
 
        // Direct observations
        if (room.HasStairsUp) logQueue.Enqueue("You see a flight of stairs heading up.");
        if (room.HasStairsDown) logQueue.Enqueue("You see a flight of stairs heading down.");
        if (room.HasBats) logQueue.Enqueue("The bats here flap and screech as you enter!");
        if (room.HasPit) logQueue.Enqueue("There is no floor here!");

        // Environmental cues from exits
        int pitCount = room.Exits.Count(e => e.HasPit);
        int batCount = room.Exits.Count(e => e.HasBats);
        bool hasWumpus = room.Exits.Any(e => e.HasWumpus);
        if (hasWumpus) logQueue.Enqueue("You smell something terrible nearby.");
        if (pitCount > 0) logQueue.Enqueue($"You feel a {(pitCount > 1 ? "strong wind" : "breeze")} nearby.");
        if (batCount > 0) logQueue.Enqueue($"You hear {(batCount > 1 ? "a cacophony of" : "some")} flapping sounds nearby.");
    }
}
