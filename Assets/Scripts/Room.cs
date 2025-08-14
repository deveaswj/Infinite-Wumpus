using System.Collections.Generic;
using UnityEngine;

public class Treasure
{
    public int Value { get; set; }
    public string Description { get; set; }
}

public class Room
{
    int id;
    public int ID => id;

    int uid;
    public int UID => uid;

    List<Room> exits = new();
    public List<Room> Exits => exits;

    public bool IsConnected() => exits.Count > 0;
    public bool IsConnected(Room room) => exits.Contains(room);
    public void Connect(Room room) => exits.Add(room);

    int level = 0;
    public int Level => level;

    private List<Actor> occupants = new List<Actor>();
    public IReadOnlyList<Actor> Occupants => occupants;
    public bool IsOccupied => occupants.Count > 0;

    public bool IsSafe => !HasPit && !HasBats;

    // Stairs up/down always lead to the same Room number on the DungeonLevel above/below this one
    // Pit always leads to the same Room number on the DungeonLevel below this one
    // With these constraints we don't have to store room number references
    public bool HasStairsUp { get; private set; } = false;
    public bool HasStairsDown { get; private set; } = false;
    public bool HasPit { get; private set; } = false;
    public bool HasDonut { get; private set; } = false;
    public bool HasBats { get; private set; } = false;
    public bool HasTreasure { get; private set; } = false;

    Treasure treasure = null;
    public Treasure Treasure { get => treasure; }

    public bool HasAnyStairs() => HasStairsUp || HasStairsDown;

    public Room(int level, int id)
    {
        this.level = level;
        this.id = id;
        uid = (level * 1000) + id; // Unique ID - level 01 room 20 = 01020, and so on - not scalable beyond 999 rooms per level
    }

    public void SetExits(Room[] rooms)
    {
        for (int i = 0; i < rooms.Length; i++)
            exits.Add(rooms[i]);
    }

    public void SetStairsUp(bool value)
    {
        HasStairsUp = value;
        // Debug.Log("Stairs up in room " + id + " set to: " + value);
    }

    public void SetStairsDown(bool value)
    {
        HasStairsDown = value;
        // Debug.Log("Stairs down in room " + id + " set to: " + value);
    }

    public void SetPit(bool value) => HasPit = value;
    public void SetDonut(bool value) => HasDonut = value;
    public void SetBats(bool value) => HasBats = value;

    // public void SetWumpus(bool value) => HasWumpus = value;

    public void SetTreasure(bool value)
    {
        if (value)
        {
            treasure = CreateTreasure();
        }
        else
        {
            treasure = null;
        }
        HasTreasure = value;
    }

    // Calling GetTreasure clears the treasure flag and returns the value
    public Treasure ClaimTreasure()
    {
        Treasure roomTreasure = treasure;
        if (roomTreasure.Value > 0) SetTreasure(false);
        return roomTreasure;
    }

    Treasure CreateTreasure()
    {
        // create a list of treasure descriptions so we can pick one
        List<Treasure> treasures = new List<Treasure>
            {
                new() { Value =  1, Description = "a copper coin" },
                new() { Value =  4, Description = "a silver coin" },
                new() { Value =  7, Description = "a few coins" },
                new() { Value = 10, Description = "several coins" },
                new() { Value = 15, Description = "a gemstone" },
                new() { Value = 20, Description = "a jeweled egg" }
            };

        // get a random treasure
        int index = Random.Range(0, treasures.Count);
        Treasure treasure = treasures[index];
        return treasure;
    }

    public void AddOccupant(Actor actor)
    {
        if (!occupants.Contains(actor))
            occupants.Add(actor);
    }

    public void RemoveOccupant(Actor actor)
    {
        occupants.Remove(actor);
    }

    public void ClearOccupants() => occupants.Clear();
}
