using UnityEngine;

public interface ILocatable
{
    int CurrentLevel { get; }
    int CurrentRoomID { get; }
}

public class Actor : ILocatable
{
    public int CurrentLevel { get; private set; }
    public int CurrentRoomID { get; private set; }
    protected Dungeon dungeon;

    public Actor(Dungeon dungeon)
    {
        this.dungeon = dungeon;
    }

    public void MoveTo(int level, int roomID)
    {
        dungeon.GetRoom(level, roomID); // Ensure exists
        CurrentLevel = level;
        CurrentRoomID = roomID;
    }
}
