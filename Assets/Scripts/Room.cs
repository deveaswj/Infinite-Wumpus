using System;
using System.Collections.Generic;

public class Room
{
    int id;
    public int ID => id;
    List<Room> exits = new();
    public List<Room> Exits => exits;

    public bool IsConnected() => exits.Count > 0;
    public bool IsConnected(Room room) => exits.Contains(room);
    public void Connect(Room room) => exits.Add(room);

    int level = 0;
    public int Level => level;

    // Stairs up/down always lead to the same Room number on the DungeonLevel above/below this one
    // Pit always leads to the same Room number on the DungeonLevel below this one
    // With these constraints we don't have to store room number references
    public bool HasStairsUp = false;
    public bool HasStairsDown = false;
    public bool HasPit = false;
    public bool HasWumpus = false;
    public bool HasTreasure = false;
    public bool Collected = false;

    public Room(int id, int level)
    {
        this.id = id;
        this.level = level;
    }

    public Room(int id)
    {
        this.id = id;
        this.level = 0;
    }

    public void SetExits(Room[] rooms)
    {
        for (int i = 0; i < rooms.Length; i++)
            exits.Add(rooms[i]);
    }
}
