using UnityEngine;

public interface ILocatable
{
    int CurrentLevel { get; }
    int CurrentRoomID { get; }
}

public abstract class Actor : ILocatable
{
    public int CurrentLevel { get; private set; }
    public int CurrentRoomID { get; private set; }
    protected Dungeon dungeon;

    public bool IsFalling { get; private set; } = false;

    public Actor(Dungeon dungeon)
    {
        this.dungeon = dungeon;
    }

    public Room MoveTo(int levelID, int roomID)
    {
        Room room = dungeon.GetRoom(levelID, roomID); // Ensure exists
        CurrentLevel = levelID;
        CurrentRoomID = roomID;
        return room;
    }

    public abstract void HandlePit();

}
