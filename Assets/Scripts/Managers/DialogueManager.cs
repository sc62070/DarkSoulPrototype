using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    public static DialogueManager instance;

    public DialogueManagerPrefabCollection prefabs;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    void Update() {

    }

    public void ShowDialogue(string text) {
        
    }
}

[System.Serializable]
public class DialogueManagerPrefabCollection {
    public GameObject dialog;
}