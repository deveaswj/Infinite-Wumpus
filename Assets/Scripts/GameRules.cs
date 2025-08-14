using UnityEngine;

// Responsible for enforcing game rules
public class GameRules
{
    private Dungeon dungeon;
    private Player player;
    private RollingTextHistory narrator;

    public GameRules(Dungeon dungeon, Player player, RollingTextHistory narrator)
    {
        this.dungeon = dungeon;
        this.player = player;
        this.narrator = narrator;
    }

    public void OnActorEnterRoom(Actor actor)
    {
        var room = dungeon.GetRoom(actor);

        if (room.HasPit)
        {
            actor.HandlePit();
            // actor.TakeDamage(10);
            // actor.MoveTo(actor.CurrentLevel + 1, actor.CurrentRoomId);
            // OnActorEnterRoom(actor); // recursive fall until no pit
            return;
        }

        // Player-only instant pickups
        if (actor is Player player)
        {
            if (room.HasTreasure)
            {
                // actor.HandleTreasure();
                Treasure treasure = room.GetTreasure();
                narrator.Say($"You find {treasure.Description} in the room.");
                player.AddScore(treasure.Value);
            }
        }
    }
}
