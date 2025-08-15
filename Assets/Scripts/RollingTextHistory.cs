using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;

public class RollingTextHistory : MonoBehaviour
{
    public TMP_Text historyText;
    public int maxEntries = 500;

    private Queue<string> entryQueue = new();
    private StringBuilder displayBuilder = new();

    void Awake()
    {
        ClearHistory();
    }

    void Start()
    {
        // ClearHistory();
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

    public void Say(StringBuilder newEntry)
    {
        Say(newEntry.ToString());
    }

    public void Say(Queue<string> newEntries)
    {
        string entry = "";

        // Dequeue newEntries and Say each one
        while (newEntries.Count > 0)
        {
            entry = newEntries.Dequeue();
            Debug.Log("Say dequeued entry: " + entry);
            Say(entry);
        }

        // // this doesn't dequeue newEntries, but that's fine
            // foreach (string entry in newEntries)
            // {
            //     Debug.Log("Say queued entry: " + entry);
            //     Say(entry);
            // }
        }

    public void ClearHistory()
    {
        // Debug.Log("Clearing text history");
        entryQueue.Clear();
        displayBuilder.Clear();
        historyText.text = "";
    }
}

