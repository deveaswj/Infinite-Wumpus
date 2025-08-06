public class PlayerState
{
    public int Health = 3;
    public int Score = 0;

    public bool HasDonut = false;

    public int CurrentLevel = 0;
    public int CurrentRoomID = 0;

    // For climbing back up from stairs
    public Stack<(int level, int room)> RoomHistory = new();

    public void MoveTo(int roomID)
    {
        CurrentRoomID = roomID;
    }

    public void FallTo(int newLevel, int newRoomID)
    {
        CurrentLevel = newLevel;
        CurrentRoomID = newRoomID;
        Health = Mathf.Max(Health - 1, 0);
        RoomHistory.Clear(); // No going back after a fall
    }

    public void ClimbTo(int newLevel, int newRoomID)
    {
        RoomHistory.Push((CurrentLevel, CurrentRoomID));
        CurrentLevel = newLevel;
        CurrentRoomID = newRoomID;
    }

    public bool CanClimbBack()
    {
        return RoomHistory.Count > 0;
    }

    public void ClimbBack()
    {
        if (CanClimbBack())
        {
            var (level, room) = RoomHistory.Pop();
            CurrentLevel = level;
            CurrentRoomID = room;
        }
    }

    public void CollectTreasure()
    {
        Score += 1;
    }

    public void UseDonut()
    {
        if (HasDonut)
        {
            HasDonut = false;
            Health = Mathf.Min(Health + 1, 3);
        }
    }
}
