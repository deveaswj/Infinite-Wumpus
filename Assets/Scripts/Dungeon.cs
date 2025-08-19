using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dungeon
{
    private List<DungeonLevel> levels = new();
    // private Room currentRoom;

    public int Seed { get; private set; } = 0;

    public Dungeon() : this(0) { }

    public Dungeon(int dungeonSeed)
    {
        Seed = dungeonSeed;

        if (Seed == 0)
            Seed = Random.Range(int.MinValue, int.MaxValue);

        Random.InitState(Seed);
        Debug.Log("Dungeon seed: " + Seed);

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

    public Room GetRoom(Actor actor) => GetRoom(actor.CurrentLevel, actor.CurrentRoomID);
    public Room GetRoom(int levelID, int roomID)
    {
        EnsureLevelExists(levelID);
        return levels[levelID].GetRoom(roomID);
    }

    public DungeonLevel GetLevel(Actor actor) => GetLevel(actor.CurrentLevel);
    public DungeonLevel GetLevel(int levelID) => levels[levelID];

    private void AddNewLevel()
    {
        EnsureLevelExists(levels.Count);
    }

    private DungeonLevel GenerateLevel(int levelID)
    {
        Debug.Log("Generating level " + levelID);

        DungeonLevel newLevel = new DungeonLevel(levels.Count - 1);

        int roomCount = newLevel.RoomCount();

        DungeonLevel aboveLevel = null;
        if (levelID > 0)
            aboveLevel = levels[levelID - 1];

        // Link stairs up if not first level
        if (levelID > 0 && aboveLevel != null)
        {
            int upId = aboveLevel.StairsDownRoomID;
            newLevel.SetStairsUp(upId, true);
        }

        // Pick new level's stairs-down room
        // (may or may not be the same room as the stairs-up room)
        int stairsRoomId = Random.Range(0, roomCount);
        newLevel.SetStairsDown(stairsRoomId, true);

        List<int> aboveLevelPitRoomIDs = new();
        if (aboveLevel != null)
            aboveLevelPitRoomIDs = aboveLevel.PitRoomIDs;

        // Add features
        newLevel.AddPits(aboveLevelPitRoomIDs);
        newLevel.AddFeatures();

        return newLevel;
    }
}
