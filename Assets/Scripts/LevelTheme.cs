using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelTheme", menuName = "Scriptable Objects/LevelTheme")]
public class LevelTheme : ScriptableObject
{
    public string themeName;

    [TextArea(2, 5)] public List<string> roomDescriptions;
    [TextArea(2, 5)] public List<string> exitDescriptions;

    public UIStyle uiStyle;

    void OnEnable()
    {
        if (roomDescriptions.Count == 0) roomDescriptions.Add("A generic room with stone walls and a few exits.");
        if (exitDescriptions.Count == 0) exitDescriptions.Add("Room #ROOM#");
    }

}