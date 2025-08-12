using UnityEngine;

// Responsible for enforcing game rules
public class GameRules
{
    private Dungeon dungeon;

    public GameRules(Dungeon dungeon)
    {
        this.dungeon = dungeon;
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

        if (room.HasTreasure)
        {
            actor.HandleTreasure();
        }
    }
}
