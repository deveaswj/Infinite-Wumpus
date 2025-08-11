using UnityEngine;

public class NPC : Actor
{
    public NPC(Dungeon dungeon) : base(dungeon) {}

    public override void HandleDonut(Room room)
    {
        // do nothing
    }

    public override void HandlePit(Room room)
    {
        // do nothing
    }

    public override void HandleTreasure(Room room)
    {
        // do nothing
    }

    public override void HandleWumpus(Room room)
    {
        // do nothing
    }
}
