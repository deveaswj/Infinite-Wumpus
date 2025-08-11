using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dungeon
{
    [SerializeField] Player player;

    public int CurrentLevel => player.CurrentLevel;
    private List<DungeonLevel> levels = new();
    // private Room currentRoom;

    public Dungeon()
    {
        AddNewLevel(); // Start with one level
    }

    private void EnsureLevelExists(int targetLevel)
    {
        // Make sure the list can hold the target level index
        while (levels.Count <= targetLevel)
            levels.Add(null);

        // Generate any missing levels sequentially so links are correct
        for (int i = 0; i <= targetLevel; i++)
        {
            if (levels[i] == null)
                levels[i] = GenerateLevel(i);
        }
    }

    public Room GetRoom(int levelNumber, int roomId)
    {
        EnsureLevelExists(levelNumber);
        return levels[levelNumber].GetRoom(roomId);
    }

    public Room GetRoom(Actor actor) => GetRoom(actor.CurrentLevel, actor.CurrentRoomID);

    public Room GetCurrentRoom()
    {
        return GetRoom(player);
    }

    private void AddNewLevel()
    {
        EnsureLevelExists(levels.Count);
    }

    private DungeonLevel GenerateLevel(int levelNumber)
    {
        Debug.Log("Generating level " + levelNumber);

        DungeonLevel newLevel = new DungeonLevel(levels.Count);

        int roomCount = newLevel.RoomCount();

        // Link stairs up if not first level
        if (levelNumber > 0 && levels[levelNumber - 1] != null)
        {
            int upId = levels[levelNumber - 1].StairsDownRoomID;
            newLevel.SetStairsUp(upId, true);
        }

        // Pick new level's stairs-down room
        // (may or may not be the same room as the stairs-up room)
        int stairsRoomId = Random.Range(0, roomCount);
        newLevel.SetStairsDown(stairsRoomId, true);

        // Add features

        // Add pits
        newLevel.AddPits();

        // Add treasures
        newLevel.AddTreasures();

        // Add donuts
        newLevel.AddDonuts();

        // Add wumpus
        newLevel.AddWumpus();

        return newLevel;
    }
}
