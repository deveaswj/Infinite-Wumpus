using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class RollingTextHistory : MonoBehaviour
{
    public TMP_Text historyText;
    public int maxEntries = 500;

    private Queue<string> entryQueue = new();
    private StringBuilder displayBuilder = new();

    void Start()
    {
        Clear();
    }

    public void Say(string newEntry)
    {
        if (entryQueue.Count >= maxEntries)
            entryQueue.Dequeue(); // remove oldest

        entryQueue.Enqueue(newEntry);

        // Rebuild display text
        displayBuilder.Clear();
        foreach (var entry in entryQueue)
            displayBuilder.AppendLine(entry);

        historyText.text = displayBuilder.ToString();
    }

    public void Clear()
    {
        entryQueue.Clear();
        displayBuilder.Clear();
        historyText.text = "";
    }
}

