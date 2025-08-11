using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int Health = 3;
    public int Score = 0;
    public int TreasureCount = 0;

    public bool HasDonut = false;

    int currentLevel = 0;
    int currentRoomID = 0;

    public int CurrentLevel => currentLevel;
    public int CurrentRoomID => currentRoomID;

    private Dungeon dungeon;

    public void EnterDungeon(Dungeon newDungeon)
    {
        dungeon = newDungeon;
        MoveTo(0, 0);
    }

    public void MoveTo(int roomID)
    {
        currentRoomID = roomID;
    }

    public void MoveTo(int roomID, int level)
    {
        dungeon.GetRoom(level, roomID);
        currentRoomID = roomID;
        currentLevel = level;
    }

    public void Ascend()
    {
        MoveTo(currentRoomID, currentLevel - 1);
    }

    public void Descend()
    {
        MoveTo(currentRoomID, currentLevel + 1);
    }

    public void FallTo(int newLevel, int newRoomID)
    {
        currentLevel = newLevel;
        currentRoomID = newRoomID;
        Health = Mathf.Max(Health - 1, 0);
    }

    public void TakeDamage()
    {
        Health = Mathf.Max(Health - 1, 0);
    }

    public bool IsDead() => Health == 0;

    public void CollectTreasure()
    {
        // current room SetTreasure(false)
        TreasureCount += 1;
        dungeon.GetCurrentRoom().SetTreasure(false);
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
