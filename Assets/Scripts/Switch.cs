using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Interactable {

    private string[] actions = { "Switch" };
    public override string[] Actions { get => actions; set => actions = value; }

    public Switchable switchable;

    public override void DoSelectedAction() {
        if(switchable != null) {
            switchable.Switch();
        }
    }

    void Start() {

    }

    new void Update() {
        base.Update();
    }
}