using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchableLight : Switchable {

    private bool state = false;
    public override bool State { get => state; }

    Light light;

    void Awake() {
        light = GetComponent<Light>();
        state = light.enabled;
    }

    public override void Switch() {
        state = !state;
        light.enabled = state;
    }
}
