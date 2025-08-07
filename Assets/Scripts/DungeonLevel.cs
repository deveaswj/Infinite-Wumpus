using Unity.VisualScripting;
using UnityEngine;

public class DungeonLevel
{
    const int NUM_ROOMS = 20;

    int levelNumber;
    public int LevelNumber => levelNumber;

    Room[] rooms = new Room[NUM_ROOMS];
    // public Room[] Rooms => rooms;

    public DungeonLevel(int levelNum)
    {
        levelNumber = levelNum;
        GenerateLevel();
    }

    void GenerateLevel()
    {
        CreateRooms();
        AssignExits();
        AddFeatures();
        // We'll assign stairs later, based on inter-level generation
    }

    void AddFeatures()
    {
        // Add hazards & features (random for now)
        int pitRoom = Random.Range(0, NUM_ROOMS);
        rooms[pitRoom].HasPit = true;

        int treasureRoom = (pitRoom + 3) % NUM_ROOMS;
        rooms[treasureRoom].HasTreasure = true;

        int wumpusRoom = (pitRoom + 6) % NUM_ROOMS;
        rooms[wumpusRoom].HasWumpus = true;
    }

    public void AddUpStairs(int roomID) => rooms[roomID].HasStairsUp = true;
    public void AddDownStairs(int roomID) => rooms[roomID].HasStairsDown = true;

    void CreateRooms()
    {
        // Create NUM_ROOMS rooms
        for (int i = 0; i < NUM_ROOMS; i++)
            rooms[i] = new Room(i, levelNumber);
    }

    void AssignExits()
    {
        // Dodecahedron hardcoded connections
        int[][] neighborMap = new int[][]
        {
            new[] {1, 4, 7},    //0
            new[] {0, 2, 9},    //1
            new[] {1, 3, 11},   //2
            new[] {2, 4, 13},   //3
            new[] {0, 3, 5},    //4
            new[] {4, 6, 14},   //5
            new[] {5, 7, 16},   //6
            new[] {0, 6, 8},    //7
            new[] {7, 9, 17},   //8
            new[] {1, 8, 10},   //9
            new[] {9, 11, 18},  //10
            new[] {2, 10, 12},  //11
            new[] {11, 13, 19}, //12
            new[] {3, 12, 14},  //13
            new[] {5, 13, 15},  //14
            new[] {14, 16, 19}, //15
            new[] {6, 15, 17},  //16
            new[] {8, 16, 18},  //17
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

    public Room GetRoom(int id) => rooms[id];
}
