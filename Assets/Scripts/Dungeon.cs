using System.Collections.Generic;

public class Dungeon
{
    public int CurrentLevel => levelStack.Count - 1;
    private List<Room[]> levelStack = new List<Room[]>();
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

    public void GoDownstairs(Room fromRoom)
    {
        if (CurrentLevel == levelStack.Count - 1)
            AddNewLevel();

        Room[] nextLevel = levelStack[CurrentLevel + 1];
        Room targetRoom = nextLevel[fromRoom.ID];
        targetRoom.HasStairsUp = true;
        currentRoom = targetRoom;
    }

    public void GoUpstairs(Room fromRoom)
    {
        if (CurrentLevel > 0)
        {
            Room[] prevLevel = levelStack[CurrentLevel - 1];
            Room targetRoom = prevLevel[fromRoom.ID];
            currentRoom = targetRoom;
        }
    }

    public void FallToNextLevel()
    {
        if (CurrentLevel == levelStack.Count - 1)
            AddNewLevel();

        Room[] nextLevel = levelStack[CurrentLevel + 1];
        currentRoom = nextLevel[currentRoom.ID];
    }

    private void AddNewLevel()
    {
        Room[] newLevel = new Room[12];
        for (int i = 0; i < 12; i++)
            newLevel[i] = new Room(i);

        // Basic room connection: each room connects to 3 others
        for (int i = 0; i < 12; i++)
        {
            Room r = newLevel[i];
            r.Exits.Add(newLevel[(i + 1) % 12]);
            r.Exits.Add(newLevel[(i + 5) % 12]);
            r.Exits.Add(newLevel[(i + 9) % 12]);
        }

        // Place one stair down and one treasure
        newLevel[0].HasStairsDown = true;
        newLevel[3].HasTreasure = true;

        // Random pit and Wumpus placement (later we'll want to make this smarter)
        newLevel[7].IsPit = true;
        newLevel[10].HasWumpus = true;

        levelStack.Add(newLevel);
        currentRoom = newLevel[0]; // Start new level in room 0
    }
}
