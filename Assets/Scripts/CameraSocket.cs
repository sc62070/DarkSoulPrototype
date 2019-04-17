using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSocket : MonoBehaviour {

    public HashSet<GameObject> listeners;

    void Start() {

    }

    void Update() {

    }

    public void FirePositionChanged() {
        foreach (GameObject o in listeners) {
            o.SendMessage("OnPositionChanged");
        }
    }
}
