using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Interactable {

    private string description = "Switch";
    public override string Description { get => description; set => description = value; }

    public override bool IsInteractable => true;

    public Switchable switchable;

    public override void Interact(Character character) {
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