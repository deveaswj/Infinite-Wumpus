using UnityEngine;

public class Wumpus
{
    const int NUM_ROOMS = 20;

    public int RoomID { get; private set; }

    public Wumpus(int roomID) => RoomID = roomID;
    public Wumpus() => RoomID = -1;

    public void SetRoomID(int roomID)
    {
        // if between 0 and NUM_ROOMS
        if (roomID >= 0 && roomID < NUM_ROOMS)
        {
            RoomID = roomID;
        }
    }

    public void MoveTo(int roomID) => RoomID = roomID;
    public void Kill() => RoomID = -1;

    public bool IsAlive() => RoomID != -1;
    public bool IsDead() => RoomID == -1;

    public bool IsInRoom(int roomID) => RoomID == roomID;
    public bool IsInRoom(Room room) => RoomID == room.ID;

}


