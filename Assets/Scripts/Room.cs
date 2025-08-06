using System.Collections.Generic;

public class Room
{
    public int ID;
    public List<Room> Exits = new List<Room>();

    public bool HasStairsUp = false;
    public bool HasStairsDown = false;
    public bool IsPit = false;
    public bool HasWumpus = false;
    public bool HasTreasure = false;
    public bool Collected = false;

    public Room(int id)
    {
        ID = id;
    }
}
