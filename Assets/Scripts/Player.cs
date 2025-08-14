using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    const int MAX_HEALTH = 3;

    public int Score = 0;
    public int TreasureCount = 0;

    public bool HasDonut = false;

    public Player(Dungeon dungeon) : base(dungeon)
    {
        MaxHealth = MAX_HEALTH;
        Health = MAX_HEALTH;
    }

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

    public int AddScore(int amount)
    {
        Score += amount;
        return Score;
    }

    public void GainDonut() => HasDonut = true;
    public void LoseDonut() => HasDonut = false;

    public void UseDonut()
    {
        if (HasDonut)
        {
            LoseDonut();
            Health = Mathf.Min(Health + 1, MAX_HEALTH);
        }
    }

    // public void CollectDonut()
    // {
    //     // don't attempt to collect donut if you're already carrying one
    //     if (HasDonut) return;

    //     // if the current room has a donut, collect it
    //     Room currentRoom = dungeon.GetRoom(this);
    //     if (currentRoom.HasDonut)
    //     {
    //         HasDonut = true;
    //         currentRoom.SetDonut(false);
    //     }
    // }

}
