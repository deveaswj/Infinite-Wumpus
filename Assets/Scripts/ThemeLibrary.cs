using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ThemeLibrary : MonoBehaviour
{
    public List<LevelTheme> themes;

    private Dictionary<string, LevelTheme> themeDict;

    void Awake()
    {
        themeDict = themes.ToDictionary(t => t.themeName.ToLower(), t => t);
    }

    public string GetRoomDescription(string themeName)
    {
        string text = "A generic room with stone walls.";
        themeName = themeName.ToLower();
        if (themeDict.TryGetValue(themeName, out var theme))
        {
            text = GetRandom(theme.roomDescriptions);
        }
        return text;
    }

    public string GetExitDescription(string themeName)
    {
        // Whatever calls this is responsible for text-replacing #ROOM# with the actual room number.
        string text = "Room #ROOM#";
        themeName = themeName.ToLower();
        if (themeDict.TryGetValue(themeName, out var theme))
        {
            text = GetRandom(theme.exitDescriptions);
        }
        return text;
    }

    private string GetRandom(List<string> list)
    {
        return list != null && list.Count > 0 ? list[Random.Range(0, list.Count)] : "Nothing of note.";
    }
}
