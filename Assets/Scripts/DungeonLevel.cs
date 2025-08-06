using System.Collections.Generic;
using UnityEngine;

public class DungeonLevel
{
    public int LevelNumber;
    public Room[] Rooms = new Room[12];

    public DungeonLevel(int levelNum)
    {
        LevelNumber = levelNum;
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Create 12 rooms
        for (int i = 0; i < 12; i++)
            Rooms[i] = new Room(i);

        // Dodecahedron hardcoded connections (you can tweak this to generate them differently)
        int[][] neighborMap = new int[][]
        {
            new[] { 1, 4, 5 },
            new[] { 0, 2, 7 },
            new[] { 1, 3, 9 },
            new[] { 2, 4, 11 },
            new[] { 0, 3, 6 },
            new[] { 0, 7, 10 },
            new[] { 4, 8, 11 },
            new[] { 1, 5, 8 },
            new[] { 6, 7, 9 },
            new[] { 2, 8, 10 },
            new[] { 5, 9, 11 },
            new[] { 3, 6, 10 }
        };

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 3; j++)
                Rooms[i].Neighbors[j] = Rooms[neighborMap[i][j]];
        }

        // Add hazards & features (random for now)
        int pitRoom = Random.Range(0, 12);
        Rooms[pitRoom].HasPit = true;

        int treasureRoom = (pitRoom + 3) % 12;
        Rooms[treasureRoom].HasTreasure = true;

        int wumpusRoom = (pitRoom + 6) % 12;
        Rooms[wumpusRoom].HasWumpus = true;

        // We'll assign stairs later, based on inter-level generation
    }

    public Room GetRoom(int id) => Rooms[id];
}
