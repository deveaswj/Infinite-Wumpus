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

    public int MaxHealth = 1;
    private int health = 1;
    public int Health { get => health; set => health = Mathf.Clamp(value, 0, MaxHealth); }

    public Actor(Dungeon dungeon)
    {
        this.dungeon = dungeon;
    }

    public Room MoveTo(Room room) => MoveTo(room.Level, room.ID);

    public Room MoveTo(int levelID, int roomID)
    {
        Room room = dungeon.GetRoom(levelID, roomID); // Ensure exists
        CurrentLevel = levelID;
        CurrentRoomID = roomID;
        Debug.Log("MoveTo -> level " + levelID + ", room " + roomID);
        return room;
    }

    public void Fall() => FallTo(CurrentLevel + 1);
    public void FallTo(int newLevel) => FallTo(newLevel, CurrentRoomID);
    public void FallTo(int newLevel, int newRoomID)
    {
        MoveTo(newLevel, newRoomID);
        TakeDamage();
    }

    public void TakeDamage() => TakeDamage(1);
    public void TakeDamage(int amount) => Health = Mathf.Max(Health - amount, 0);

    public void Heal() => Heal(1);
    public void Heal(int amount) => Health = Mathf.Min(Health + amount, MaxHealth);

    public bool IsInjured() => Health < MaxHealth;

    public bool IsDead() => Health == 0;

}
