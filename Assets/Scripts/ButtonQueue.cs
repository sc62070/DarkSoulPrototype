using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonQueue {

    public List<QueuedButton> inputQueue = new List<QueuedButton>();

    void Start() {

    }

    public void Update() {

        // Check for input
        string[] queuableButtons = { "Circle", "Cross", "Square", "R1", "R2" };
        foreach (string btn in queuableButtons) {
            if (Input.GetButtonDown(btn)) {
                inputQueue.Add(new QueuedButton() {
                    name = btn,
                    timeRemaining = 1f
                });
            }
        }

        // Increase time counter and check if they have time remaining
        List<QueuedButton> toRemove = new List<QueuedButton>();
        foreach (QueuedButton qb in inputQueue) {
            qb.timeRemaining -= Time.deltaTime;
            if (qb.timeRemaining <= 0f) {
                toRemove.Add(qb);
            }
        }

        // Remove those queued buttons that dont have any time remaining
        foreach (QueuedButton qb in toRemove) {
            inputQueue.Remove(qb);
        }
    }

    public bool Consume(string buttonName) {

        bool match = false;

        QueuedButton toConsume = null;
        foreach (QueuedButton qb in inputQueue) {
            if (qb.name.Equals(buttonName)) {
                toConsume = qb;
                match = true;
                break;
            }
        }

        inputQueue.Remove(toConsume);

        return match;
    }
}

[System.Serializable]
public class QueuedButton {
    public string name;
    public float timeRemaining;
}
