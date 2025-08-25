// Responsible for enforcing game rules
using System.Linq;

public class GameRules
{
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
        narrator.Say("You fall into a pit.");
        player.Fall();
        if (player.IsDead()) return true;

        showInjuryMessage = true;
        narrator.Say($"You find yourself in level {player.CurrentLevel}, room {player.CurrentRoomID}.");
        OnPlayerEnterRoom(player); // recursion okay here, but safe as only one fall occurs
        return true;
    }

    private void HandleWumpus(Player player, Room room)
    {
        if (!room.HasWumpus) return;

        if (dungeon.GetLevel(player).WumpusState == WumpusState.Asleep)
        {
            narrator.Say("The Wumpus here is curled up and snoring loudly.");
            return;
        }

        if (player.HasDonut)
        {
            narrator.Say("The Wumpus grabs your donut and runs away!");
            player.LoseDonut();
        }
        else
        {
            narrator.Say("The Wumpus shrieks hungrily and attacks you!");
            player.TakeDamage(1);
            if (!player.IsDead())
            {
                narrator.Say("By the time you recover, the Wumpus is gone.");
                showInjuryMessage = true;
            }
        }
        dungeon.GetLevel(player).MoveWumpus();
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
