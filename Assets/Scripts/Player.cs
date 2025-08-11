using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    const int MAX_HEALTH = 3;

    public int Health { get; private set; } = MAX_HEALTH;
    public int Score = 0;
    public int TreasureCount = 0;

    public bool HasDonut = false;

    public Player(Dungeon dungeon) : base(dungeon) { }

    public void EnterDungeon()
    {
        MoveTo(0, 0);
    }

    public void Ascend()
    {
        MoveTo(CurrentLevel - 1, CurrentRoomID);
    }

    public void Descend()
    {
        MoveTo(CurrentLevel + 1, CurrentRoomID);
    }

    public void FallTo(int newLevel) => FallTo(newLevel, CurrentRoomID);

    public void FallTo(int newLevel, int newRoomID)
    {
        MoveTo(newLevel, newRoomID);
        TakeDamage();
    }

    public void TakeDamage() => TakeDamage(1);
    public void TakeDamage(int amount) => Health = Mathf.Max(Health - amount, 0);

    public void Heal() => Heal(1);
    public void Heal(int amount) => Health = Mathf.Min(Health + amount, MAX_HEALTH);

    public bool IsDead() => Health == 0;

    public void CollectTreasure()
    {
        // current room SetTreasure(false)
        TreasureCount += 1;
        dungeon.GetRoom(this).SetTreasure(false);
        Score += 1;
    }

    public void UseDonut()
    {
        if (HasDonut)
        {
            HasDonut = false;
            Health = Mathf.Min(Health + 1, MAX_HEALTH);
        }
    }

    public void CollectDonut()
    {
        // don't attempt to collect donut if you're already carrying one
        if (HasDonut) return;

        // if the current room has a donut, collect it
        Room currentRoom = dungeon.GetRoom(this);
        if (currentRoom.HasDonut)
        {
            HasDonut = true;
            currentRoom.SetDonut(false);
        }
    }

    public override void HandlePit(Room room)
    {
        // FallTo(CurrentLevel, CurrentRoomID);
    }

    public override void HandleTreasure(Room room)
    {
        CollectTreasure();
    }

    public override void HandleDonut(Room room)
    {
        CollectDonut();
    }

    public override void HandleWumpus(Room room)
    {
        throw new System.NotImplementedException();
    }
}
