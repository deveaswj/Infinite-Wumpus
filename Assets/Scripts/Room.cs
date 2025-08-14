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


    // Stairs up/down always lead to the same Room number on the DungeonLevel above/below this one
    // Pit always leads to the same Room number on the DungeonLevel below this one
    // With these constraints we don't have to store room number references
    public bool HasStairsUp { get; private set; } = false;
    public bool HasStairsDown { get; private set; } = false;
    public bool HasPit { get; private set; } = false;
    public bool HasDonut { get; private set; } = false;
    public bool HasTreasure { get; private set; } = false;

    private int treasureValue = 0;
    private string treasureDescription = "Nothing of note.";
    Treasure treasure = null;

    public bool HasAnyStairs() => HasStairsUp || HasStairsDown;

    public Room(int level, int id)
    {
        this.level = level;
        this.id = id;
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
    public Treasure GetTreasure()
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
                new Treasure { Value =  5, Description = "a small coin" },
                new Treasure { Value =  6, Description = "" },
                new Treasure { Value =  7, Description = "" },
                new Treasure { Value =  8, Description = "" },
                new Treasure { Value =  9, Description = "" },
                new Treasure { Value = 10, Description = "" }
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
