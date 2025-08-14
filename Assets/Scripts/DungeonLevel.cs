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

    public DungeonLevel(int levelID)
    {
        ID = levelID;
        GenerateLevel();
    }

    public Room GetRoom(int id) => rooms[id];
    public int RoomCount() => NUM_ROOMS;

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
        AddPits();
        AddTreasures();
        AddDonuts();
    }

    public void AddPits()
    {
        // Add pits to 1-3 rooms that have no stairs and no pit
        int numPits = Random.Range(1, 4);
        while (numPits > 0)
        {
            int pitRoomId = Random.Range(0, NUM_ROOMS);
            Room pitRoom = GetRoom(pitRoomId);
            if (!pitRoom.HasAnyStairs() && !pitRoom.HasPit)
            {
                Debug.Log("Level " + ID + ": Adding pit to room " + pitRoomId);
                pitRoom.SetPit(true);
                numPits--;
            }
        }
    }

    public void AddTreasures()
    {
        // Add treasures to 1-3 rooms that have no pit and no treasure
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
        // Add donuts to 0-3 rooms that have no pit and no donut
        int numDonuts = Random.Range(0, 4);
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

}
