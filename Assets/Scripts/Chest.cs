using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unimotion;

public class Chest : Interactable {
    public override string Description { get => "Open"; set => throw new System.NotImplementedException(); }

    public override bool IsInteractable => !open;

    public bool open = false;

    private float playerForwardOffset = 0.3f;

    public override void Interact(Character character) {
        character.transform.position = transform.position + transform.forward * playerForwardOffset + transform.up * 0.01f;
        character.transform.rotation = Quaternion.LookRotation(-transform.forward, -character.GetComponent<CharacterMotor>().GetGravity());
        character.photonView.RPC("PlayState", Photon.Pun.RpcTarget.All, "Open Chest", 0.2f);
        GetComponent<Animator>().Play("Opening");
        open = true;
    }

    private void OnDrawGizmosSelected() {
        
    }
}
