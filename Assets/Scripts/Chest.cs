using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unimotion;

public class Chest : Interactable {
    public override string Description { get => "Abrir"; set => throw new System.NotImplementedException(); }

    private float playerForwardOffset = 0.5f;

    public override void Interact(Character character) {
        character.transform.position = transform.position + transform.forward * playerForwardOffset;
        character.transform.rotation = Quaternion.LookRotation(-transform.forward, -character.GetComponent<CharacterMotor>().GetGravity());
        character.photonView.RPC("PlayState", Photon.Pun.RpcTarget.All, "Open Chest", 0.2f);
    }

    private void OnDrawGizmosSelected() {
        
    }
}
