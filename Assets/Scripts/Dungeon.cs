using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dungeon
{
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

    public Room GetRoom(int levelID, int roomID)
    {
        EnsureLevelExists(levelID);
        return levels[levelID].GetRoom(roomID);
    }

    public Room GetRoom(Actor actor) => GetRoom(actor.CurrentLevel, actor.CurrentRoomID);

    private void AddNewLevel()
    {
        EnsureLevelExists(levels.Count);
    }

    private DungeonLevel GenerateLevel(int levelID)
    {
        Debug.Log("Generating level " + levelID);

        DungeonLevel newLevel = new DungeonLevel(levels.Count - 1);

        int roomCount = newLevel.RoomCount();

        // Link stairs up if not first level
        if (levelID > 0 && levels[levelID - 1] != null)
        {
            int upId = levels[levelID - 1].StairsDownRoomID;
            newLevel.SetStairsUp(upId, true);
        }

        // Pick new level's stairs-down room
        // (may or may not be the same room as the stairs-up room)
        int stairsRoomId = Random.Range(0, roomCount);
        newLevel.SetStairsDown(stairsRoomId, true);

        // Add features
        newLevel.AddFeatures();

        return newLevel;
    }
}
