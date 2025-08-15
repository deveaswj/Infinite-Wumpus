using System.Collections.Generic;

// Responsible for enforcing game rules
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

    public void OnActorEnterRoom(Actor actor)
    {
        var room = dungeon.GetRoom(actor);

        // string roomDescription = dungeon.themeLibrary.GetRoomDescription...

        bool isPlayer = actor is Player;

        QueueRoomDescription();
        narrator.Say(logQueue.GetQueue());

        // Player-only environmental handling
        if (isPlayer)
        {
            if (room.HasPit)
            {
                narrator.Say("You fall into a pit.");
                player.Fall();
                if (player.IsDead())
                {
                    // "game over" message is handled elsewhere
                    return;
                }
                showInjuryMessage = true;
                narrator.Say($"You find yourself in level {player.CurrentLevel}, room {player.CurrentRoomID}.");
                // if (player.HasDonut)
                // {
                //     player.UseDonut();
                // }
                OnActorEnterRoom(player);
                return;
            }

            if (room.HasBats)
            {
                narrator.Say("You are swarmed by bats, and stumble around...");
                DungeonLevel playerLevel = dungeon.GetLevel(player);
                // first move the bats to a new room
                playerLevel.MoveBats(room);
                // now move player to a random safe room on the level -- no bats, no pits
                Room safeRoom = playerLevel.GetRandomSafeRoom();
                player.MoveTo(safeRoom);
                narrator.Say($"You recover in level {player.CurrentLevel}, room {player.CurrentRoomID}.");
                OnActorEnterRoom(player);
                return;
            }
        }

        // Player-only auto-eat donut if injured
        if (isPlayer && player.IsInjured())
        {
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

        // Player-only instant pickups
        if (isPlayer)
        {
            if (room.HasTreasure)
            {
                Treasure treasure = room.ClaimTreasure();
                narrator.Say($"You have found {treasure.Description}!");
                player.AddScore(treasure.Value);
            }

            if (room.HasDonut)
            {
                if (player.HasDonut)
                {
                    narrator.Say("There is a donut here, but you already have one.");
                }
                else
                {
                    narrator.Say("You have found a donut! This may come in handy.");
                    player.GainDonut();
                    room.SetDonut(false);
                }
            }
        }

        logQueue.Clear();
    }

    void QueueRoomDescription()
    {
        Room room = dungeon.GetRoom(player);

        // Direct observations
        if (room.HasStairsUp)
            logQueue.Enqueue("You see a flight of stairs heading up.");
        if (room.HasStairsDown)
            logQueue.Enqueue("You see a flight of stairs heading down.");
        if (room.HasBats)
            logQueue.Enqueue("The bats here flap and screech as you enter!");
        if (room.HasTreasure)
            // logQueue.Enqueue($"You see {room.Treasure.Description} here.");
            if (room.HasPit)
                logQueue.Enqueue("There is no floor here!");

        // Environmental cues from exits
        int pitCount = 0;
        int batCount = 0;
        foreach (var exit in room.Exits)
        {
            pitCount += exit.HasPit ? 1 : 0;
            batCount += exit.HasBats ? 1 : 0;
        }
        if (pitCount > 0)
            logQueue.Enqueue($"You feel a {(pitCount > 1 ? "strong wind" : "breeze")} nearby.");
        if (batCount > 1)
            logQueue.Enqueue($"You hear {(batCount > 1 ? "a cacophony of" : "some")} flapping sounds nearby.");
    }
}
