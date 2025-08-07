using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dungeon
{
    [SerializeField] Player player;

    public int CurrentLevel => player.CurrentLevel;
    private List<DungeonLevel> levelStack = new();
    private Room currentRoom;

    public Dungeon()
    {
        AddNewLevel(); // Start with one level
    }

    public Room GetStartingRoom()
    {
        return currentRoom;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    public void Descend(Room fromRoom)
    {
        if (CurrentLevel == levelStack.Count - 1)   // are we on the lowest level?
            AddNewLevel();

        // go to the same room in the next level
        player.Descend();

        // Room[] nextLevel = levelStack[CurrentLevel + 1];
        // Room targetRoom = nextLevel[fromRoom.ID];
        // targetRoom.HasStairsUp = true;
        // currentRoom = targetRoom;
    }

    public void Ascend(Room fromRoom)
    {
        if (CurrentLevel > 0)
        {
            player.Ascend();
            // Room[] prevLevel = levelStack[CurrentLevel - 1];
            // Room targetRoom = prevLevel[fromRoom.ID];
            // currentRoom = targetRoom;
        }
    }

    private void AddNewLevel()
    {
        DungeonLevel newLevel = new DungeonLevel(levelStack.Count);

        if (levelStack.Count > 0)   // no stairs on zeroth level
        {
            if (currentRoom.HasStairsDown)
            {
                // In the new level, place stairs back up
                newLevel.AddUpStairs(currentRoom.ID);
            }
        }

        newLevel[0].HasStairsDown = true;
        newLevel[3].HasTreasure = true;

        // Random pit and Wumpus placement (later we'll want to make this smarter)
        newLevel[7].HasPit = true;
        newLevel[10].HasWumpus = true;

        levelStack.Add(newLevel);
        currentRoom = newLevel[0]; // Start new level in room 0
    }
}
