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
        var level = dungeon.GetLevel(actor);

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
                OnActorEnterRoom(player);
                return;
            }

            //  What shall we do if there are bats AND a Wumpus in the room?
            //  For now, just prevent bats and Wumpus from coexisting in a room.

            // Wumpus handling
            if (room.HasWumpus)
            {
                if (level.WumpusState == WumpusState.Asleep)
                {
                    narrator.Say("The Wumpus here is curled up and snoring loudly.");
                }
                else
                {
                    // if player has a donut, Wumpus steals the donut
                    // otherwise, Wumpus attacks the player (injures them)
                    // either way, move the wumpus to a new room
                    if (player.HasDonut)
                    {
                        narrator.Say("The Wumpus is here! It grabs your donut and runs away with it.");
                        player.LoseDonut();
                    }
                    else    // player doesn't have a donut
                    {
                        narrator.Say("The Wumpus is here! It shrieks hungrily and attacks you!");
                        player.TakeDamage(1);
                        if (player.IsDead())
                        {
                            // "game over" message is handled elsewhere
                            return;
                        }
                        narrator.Say("By the time you recover, the Wumpus is gone.");
                        showInjuryMessage = true;
                    }
                    // move the wumpus to a new room
                    level.MoveWumpus();
                }
            }

            if (room.HasBats)
            {
                narrator.Say("You are swarmed by bats, and stumble around...");
                // first move the bats to a new room
                level.MoveBats(room);
                // now move player to a random safe room on the level -- no bats, no pits
                Room safeRoom = level.GetRandomSafeRoom();
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
        DungeonLevel level = dungeon.GetLevel(player);

        // Direct observations
        if (room.HasStairsUp)
            logQueue.Enqueue("You see a flight of stairs heading up.");
        if (room.HasStairsDown)
            logQueue.Enqueue("You see a flight of stairs heading down.");
        if (room.HasBats)
            logQueue.Enqueue("The bats here flap and screech as you enter!");
        // if (room.HasTreasure)
            // logQueue.Enqueue($"You see {room.Treasure.Description} here.");
        if (room.HasPit)
            logQueue.Enqueue("There is no floor here!");

        // Environmental cues from exits
        int pitCount = 0;
        int batCount = 0;
        bool hasWumpus = false;
        foreach (var exit in room.Exits)
        {
            if (exit.HasWumpus)
                hasWumpus = true;
            pitCount += exit.HasPit ? 1 : 0;
            batCount += exit.HasBats ? 1 : 0;
        }
        if (hasWumpus)
            logQueue.Enqueue("You smell something terrible nearby.");
        if (pitCount > 0)
            logQueue.Enqueue($"You feel a {(pitCount > 1 ? "strong wind" : "breeze")} nearby.");
        if (batCount > 0)
            logQueue.Enqueue($"You hear {(batCount > 1 ? "a cacophony of" : "some")} flapping sounds nearby.");
    }
}
