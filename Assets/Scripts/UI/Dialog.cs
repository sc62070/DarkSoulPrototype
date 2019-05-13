using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour {

    public static Dialog instance;
    public static Transform conversationTarget;
    public static Queue<string> dialogueQueue = new Queue<string>();

    [Header("References")]
    public Text text;

    public event Action OnContinue;
    public event Action OnConversationEnd;

    private void LateUpdate() {
        if (Input.GetButtonDown("Cross")) {
            Continue();
        }
    }

    public void SetText(string text) {
        transform.Find("Text").GetComponent<Text>().text = text;
    }

    public void Continue() {
        OnContinue?.Invoke();
        if(dialogueQueue.Count > 0) {
            Show(dialogueQueue.Dequeue());
        } else {
            OnConversationEnd?.Invoke();
            conversationTarget = null;
        }
        Destroy(gameObject);
    }

    public void Close() {
        Destroy(this.gameObject);
    }

    public static void Show(string text) {
        Show(text, null);
    }

    public static void Show(string text, Transform target) {
        UIManager manager = FindObjectOfType<UIManager>();
        instance = Instantiate(manager.componentReferences.conversationDialog, manager.main.transform, false).GetComponent<Dialog>();
        instance.GetComponent<Dialog>().SetText(text);

        if(target != null) {
            conversationTarget = target;
        }
    }

    public static void ShowConversation(string[] texts, Transform target) {
        foreach(string t in texts) {
            dialogueQueue.Enqueue(t);
        }
        Show(dialogueQueue.Dequeue(), target);
    }
}
