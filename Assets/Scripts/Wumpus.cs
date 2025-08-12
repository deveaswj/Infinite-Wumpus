using UnityEngine;

public class Wumpus : NPC
{
    public Wumpus(Dungeon dungeon) : base(dungeon) { }

    // For now, the Wumpus just stays in one room
    // If the player is in the same room:
    // - Does the player have a Donut? Yes: Wumpus steals donut and flees. No: Wumpus attacks (player takes damage) and flees.
}


