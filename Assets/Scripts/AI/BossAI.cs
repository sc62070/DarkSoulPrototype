using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAI : CharacterAI {

    public new void Start() {
        base.Start();
        FindObjectOfType<UIManager>().boss = GetComponent<Character>();
        Debug.Log(GetComponent<Character>());
    }
}
