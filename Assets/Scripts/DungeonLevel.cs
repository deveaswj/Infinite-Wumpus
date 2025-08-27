using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WumpusState
{
    None,
    Asleep,     // Doesn't move; Won't respond if player enters same room (just snores)
    Idle        // Doesn't move; Will respond if player enters same room (takes donut or injures player, then flees)
}

public class DungeonLevel
{
    const int NUM_ROOMS = 20;

    int ID;

    public bool Visited { get; set; } = false;

    Room[] rooms = new Room[NUM_ROOMS];
    public Room[] Rooms => rooms;

    public int StairsDownRoomID = -1;
    public int StairsUpRoomID = -1;

    // Keep track of which rooms have pits
    // so we can prevent adding pits below pits across levels
    // (pits should only drop one level)
    List<int> pitRoomIDs = new();

    // Keep track of which rooms have bats
    // so we can MoveAllBats w/o having to look up their locations
    List<int> batRoomIDs = new();

    // the state of the wumpus (if one exists) on THIS level
    public WumpusState WumpusState = WumpusState.None;
    public int WumpusRoomID = -1;
    private int wumpusAlarm = 0;

    public DungeonLevel(int levelID)
    {
        ID = levelID;
        GenerateLevel();
    }

    public Room GetRoom(int id) => rooms[id];
    public int RoomCount() => NUM_ROOMS;

    public List<int> PitRoomIDs
    {
        get => pitRoomIDs;
    }

    public List<int> BatRoomIDs
    {
        get => batRoomIDs;
    }

    void GenerateLevel()
    {
        CreateRooms();
        AssignExits();
    }

    public void SetStairsUp(int roomID, bool value)
    {
        Debug.Log("Level " + ID + ": Setting stairs up in room " + roomID + " to " + value);
        rooms[roomID].SetStairsUp(value);
        StairsUpRoomID = roomID;
    }

    public void SetStairsDown(int roomID, bool value)
    {
        Debug.Log("Level " + ID + ": Setting stairs down in room " + roomID + " to " + value);
        rooms[roomID].SetStairsDown(value);
        StairsDownRoomID = roomID;
    }

    void CreateRooms()
    {
        // Create NUM_ROOMS rooms
        for (int i = 0; i < NUM_ROOMS; i++)
            rooms[i] = new Room(ID, i);
    }

    public void AddFeatures()
    {
        AddBats();
        AddTreasures();
        AddDonuts();
        AddWumpus();
    }

    public void AddPits(List<int> excludeRoomIDs)
    {
        // Add pits to 1-3 rooms that have no stairs and no pit, and (via excludeRoomIDs) no pit in the level above
        int numPits = Random.Range(1, 4);
        while (numPits > 0)
        {
            int pitRoomID = Random.Range(0, NUM_ROOMS);
            // loop if we're trying to put a pit in the starting room
            if (ID == 0 && pitRoomID == 0)
            {
                Debug.Log("Level " + ID + ": Pick again - Can't pit the starting room");
                continue;
            }
            // loop if pitRoomID is in excludeRoomIDs
            if (excludeRoomIDs.Contains(pitRoomID))
            {
                Debug.Log("Level " + ID + ": Pick again - Can't pit excluded room " + pitRoomID);
                continue;
            }
            Room pitRoom = GetRoom(pitRoomID);
            if (!pitRoom.HasAnyStairs() && !pitRoom.HasPit)
            {
                if (!IsSafeToPlacePit(pitRoom))
                {
                    Debug.Log("Level " + ID + ": Pick again - Isolation would occur - room " + pitRoomID);
                    continue;
                }
                Debug.Log("Level " + ID + ": Adding pit to room " + pitRoomID);
                pitRoom.SetPit(true);
                pitRoomIDs.Add(pitRoomID);
                numPits--;
            }
        }
    }

    private bool IsSafeToPlacePit(Room candidate)
    {
        // Rule 1: Candidate must not already have stairs or a pit
        if (candidate.HasAnyStairs() || candidate.HasPit)
            return false;

        // Rule 2: Placing a pit here must not isolate any neighbor
        foreach (Room neighbor in candidate.Exits)
        {
            if (neighbor.HasPit) continue; // Already isolated

            int safeExits = neighbor.Exits.Count(exit => !exit.HasPit && exit != candidate);
            if (safeExits == 0)
            {
                // This neighbor would be isolated if candidate becomes a pit
                return false;
            }
        }

        return true;
    }

    public void AddTreasures()
    {
        // Add treasures to 1-3 rooms that have no pit and no treasure
        // NOTE: It is presently theoretically possible for a room with stairs to be surrounded by rooms with pits.
        // This is something that I ought to prevent, as it makes it impossible to progress through the dungeon.
        int numTreasures = Random.Range(1, 4);
        while (numTreasures > 0)
        {
            int treasureRoomId = Random.Range(0, NUM_ROOMS);
            Room treasureRoom = GetRoom(treasureRoomId);
            if (!treasureRoom.HasPit && !treasureRoom.HasTreasure)
            {
                Debug.Log("Level " + ID + ": Adding treasure to room " + treasureRoomId);
                treasureRoom.SetTreasure(true);
                numTreasures--;
            }
        }
    }

    public void AddDonuts()
    {
        // If this level number is divisible by the divisor, always add at least one donut
        // Scale the divisor based on the level -- deeper = rarer
        int divisor = ID < 10 ? 2 : ID < 20 ? 3 : ID < 45 ? 4 : 5;
        int minDonuts = ID % divisor == 0 ? 1 : 0;
        // Add donuts to 0/1-3 rooms that have no pit and no donut
        int numDonuts = Random.Range(minDonuts, 4);
        while (numDonuts > 0)
        {
            int donutRoomId = Random.Range(0, NUM_ROOMS);
            Room donutRoom = GetRoom(donutRoomId);
            if (!donutRoom.HasPit && !donutRoom.HasDonut)
            {
                Debug.Log("Level " + ID + ": Adding donut to room " + donutRoomId);
                donutRoom.SetDonut(true);
                numDonuts--;
            }
        }
    }

    public void AddBats()
    {
        // Add bats to 1-3 rooms that have no bats
        // Bats can coexist with pits! They can fly!
        int numBats = Random.Range(1, 4);
        AddBats(numBats);
    }

    public void AddBats(int numBats)
    {
        // Add (numBats) bats to rooms that have no bats, stairs, or Wumpus
        // Bats can coexist with pits! They can fly!
        //
        // Note: this could place bats in the player's room
        //
        while (numBats > 0)
        {
            int batsRoomId = Random.Range(0, NUM_ROOMS);
            Room batsRoom = GetRoom(batsRoomId);
            if (!batsRoom.HasBats && !batsRoom.HasWumpus && !batsRoom.HasAnyStairs())
            {
                Debug.Log("Level " + ID + ": Adding bats to room " + batsRoomId);
                batsRoom.SetBats(true);
                batRoomIDs.Add(batsRoomId);
                numBats--;
            }
        }
    }

    public void MoveBats(Room fromRoom)
    {
        if (fromRoom.HasBats)
        {
            fromRoom.SetBats(false);
            batRoomIDs.Remove(fromRoom.ID);
            AddBats(1);
        }
    }

    public void MoveAllBats()
    {
        // copy batRoomIDs because it will be modified
        List<int> fromRooms = batRoomIDs.ToList();
        foreach (var roomID in fromRooms)
        {
            Room fromRoom = GetRoom(roomID);
            if (fromRoom.HasBats)
            {
                MoveBats(fromRoom);
            }
        }
    }

    public void AddWumpus()
    {
        // If this level ID is a nonzero multiple of five, add a wumpus
        // to a room that has no: pit, torch, or stairs
        // if (ID > 0 && ID % 5 == 0)
        if (ID % 5 == 0)
        {
            int wumpusRoomId = -1;
            Room wumpusRoom = null;
            bool isValidRoom = false;
            int attempts = 0;

            WumpusState = WumpusState.None;
            WumpusRoomID = wumpusRoomId;

            while (!isValidRoom && attempts < 16)
            {
                wumpusRoomId = Random.Range(0, NUM_ROOMS);
                wumpusRoom = GetRoom(wumpusRoomId);
                isValidRoom = !wumpusRoom.HasPit && !wumpusRoom.HasBats && !wumpusRoom.HasTorch && !wumpusRoom.HasAnyStairs();
                // is the player already in this room?
                // isValidRoom = isValidRoom && !player.IsInRoom(wumpusRoomId);
                // this class has no player reference, so we can't check if the player is in the room
            }
            if (isValidRoom)
            {
                // we have a valid room to place the wumpus into
                Debug.Log("Level " + ID + ": Adding wumpus to room " + wumpusRoomId);
                wumpusRoom.SetWumpus(true);
                WumpusState = WumpusState.Idle;
                WumpusRoomID = wumpusRoomId;
            }
            else
            {
                Debug.Log("Level " + ID + ": Failed to add wumpus - too many attempts");
            }
        }
        return;
    }

    public void MoveWumpus()
    {
        if (WumpusRoomID == -1)
        {
            return;
        }

        Room fromRoom = GetRoom(WumpusRoomID);
        if (fromRoom.HasWumpus)
        {
            fromRoom.SetWumpus(false);
            // Get a new room to place the wumpus into (no pit, no torch, no stairs, no player)
            AddWumpus();
        }
    }

    public void SleepWumpus(int numTurns)
    {
        if (WumpusRoomID == -1) return;
        Debug.Log("Level " + ID + ": Putting the Wumpus to sleep");
        WumpusState = WumpusState.Asleep;
        SetWumpusAlarm(numTurns);
    }

    public void WakeWumpus()
    {
        if (WumpusRoomID == -1) return;
        Debug.Log("Level " + ID + ": Waking up the Wumpus");
        WumpusState = WumpusState.Idle;
    }

    public void SetWumpusAlarm(int newAlarm)
    {
        if (WumpusRoomID == -1) return;
        wumpusAlarm = newAlarm;
    }
    public void UpdateWumpusAlarm()
    {
        if (WumpusRoomID == -1) return;
        if (wumpusAlarm > 0)
        {
            wumpusAlarm--;
            if (wumpusAlarm <= 0)
            {
                wumpusAlarm = 0;
                WakeWumpus();
            }
        }
    }

    void AssignExits()
    {
        // Dodecahedron hardcoded connections
        int[][] neighborMap = new int[][]
        {
            new[] { 1,  4,  7},    //0
            new[] { 0,  2,  9},    //1
            new[] { 1,  3, 11},   //2
            new[] { 2,  4, 13},   //3
            new[] { 0,  3,  5},    //4
            new[] { 4,  6, 14},   //5
            new[] { 5,  7, 16},   //6
            new[] { 0,  6,  8},    //7
            new[] { 7,  9, 17},   //8
            new[] { 1,  8, 10},   //9
            new[] { 9, 11, 18},  //10
            new[] { 2, 10, 12},  //11
            new[] {11, 13, 19}, //12
            new[] { 3, 12, 14},  //13
            new[] { 5, 13, 15},  //14
            new[] {14, 16, 19}, //15
            new[] { 6, 15, 17},  //16
            new[] { 8, 16, 18},  //17
            new[] {10, 17, 19}, //18
            new[] {12, 15, 18}  //19
        };

        // Assign exits
        for (int i = 0; i < NUM_ROOMS; i++)
        {
            Room[] exits = new Room[3];
            for (int j = 0; j < 3; j++)
                exits[j] = rooms[neighborMap[i][j]];

            rooms[i].SetExits(exits);
        }
    }

    public int CountTreasures()
    {
        int count = 0;
        for (int i = 0; i < NUM_ROOMS; i++)
        {
            if (rooms[i].HasTreasure) count++;
        }
        return count;
    }

    public Room GetRandomRoom() => rooms[Random.Range(0, NUM_ROOMS)];

    public Room GetRandomSafeRoom()
    {
        Room randomRoom = GetRandomRoom();
        while (!randomRoom.IsSafe())
            randomRoom = GetRandomRoom();
        Debug.Log("GetRandomSafeRoom: Room " + randomRoom.ID + (randomRoom.IsSafe() ? " is safe" : " is not safe"));
        return randomRoom;
    }

}
