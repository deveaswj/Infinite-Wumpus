using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonLevel
{
    const int NUM_ROOMS = 20;

    int ID;

    Room[] rooms = new Room[NUM_ROOMS];
    public Room[] Rooms => rooms;

    public int StairsDownRoomID = -1;
    public int StairsUpRoomID = -1;

    // Keep track of which rooms have pits
    // so we can prevent adding pits below pits across levels
    // (pits should only drop one level)
    List<int> pitRoomIDs = new();

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
        // Scale the divisor: Up to level 10, divisor is 3, up to level 20, divisor is 4, beyond 30, divisor is 5
        //
        //
        //
        int divisor = (ID < 10 ? 3 : ID < 20 ? 4 : 5);
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
        // Add (numBats) bats to rooms that have no bats nor stairs
        // Bats can coexist with pits! They can fly!
        while (numBats > 0)
        {
            int batsRoomId = Random.Range(0, NUM_ROOMS);
            Room batsRoom = GetRoom(batsRoomId);
            if (!batsRoom.HasBats && !batsRoom.HasAnyStairs())
            {
                Debug.Log("Level " + ID + ": Adding bats to room " + batsRoomId);
                batsRoom.SetBats(true);
                numBats--;
            }
        }
    }

    public void MoveBats(Room fromRoom)
    {
        if (fromRoom.HasBats)
        {
            fromRoom.SetBats(false);
            AddBats(1);
        }
    }


    public void AddWumpus()
    {
        //
        //  #TODO
        //
        return;
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
