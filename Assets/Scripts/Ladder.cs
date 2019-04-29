using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Interactable {
    public override string Description { get => "Climb"; set => throw new System.NotImplementedException(); }

    public float height = 4f;

    public override void Interact() {

    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * height);
    }
}
