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
    public bool HasStairsUp { get; private set; } = false;
    public bool HasStairsDown { get; private set; } = false;
    public bool HasPit { get; private set; } = false;
    public bool HasDonut { get; private set; } = false;
    public bool HasWumpus { get; private set; } = false;
    public bool HasTreasure { get; private set; } = false;

    public bool IsSafe() => !HasPit && !HasWumpus;
    public bool HasAnyStairs() => HasStairsUp || HasStairsDown;

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

    public void SetStairsUp(bool value) => HasStairsUp = value;
    public void SetStairsDown(bool value) => HasStairsDown = value;
    public void SetPit(bool value) => HasPit = value;
    public void SetDonut(bool value) => HasDonut = value;
    // public void SetWumpus(bool value) => HasWumpus = value;
    public void SetTreasure(bool value) => HasTreasure = value;
}
