using System.Collections.Generic;
using UnityEngine;

public class StringQueue
{
    public int maxLines = 500;
    private Queue<string> lineQueue = new();

    public StringQueue() { }

    public StringQueue(int maxLines)
    {
        this.maxLines = maxLines;
    }

    public void TrimQueue() => TrimQueue(maxLines);
    public void TrimQueue(int maxLines)
    {
        this.maxLines = maxLines;
        while (lineQueue.Count > maxLines)
            lineQueue.Dequeue();
    }

    public void Enqueue(StringQueue queue)
    {
        while (queue.lineQueue.Count > 0)
            Enqueue(queue.lineQueue.Dequeue());
    }

    public void Enqueue(string newLine)
    {
        if (lineQueue.Count >= maxLines)
            lineQueue.Dequeue(); // remove oldest

        lineQueue.Enqueue(newLine);
    }

    public Queue<string> GetQueue()
    {
        Debug.Log("GetQueue: " + lineQueue.Count + " lines");
        return lineQueue;
    }

    public void Clear()
    {
        lineQueue.Clear();
    }
}
