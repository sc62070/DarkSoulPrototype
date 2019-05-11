using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talkable : Interactable
{
    public override string Description { get => "Talk"; set => throw new System.NotImplementedException(); }

    public override bool IsInteractable => Dialog.instance == null;

    public string[] conversationTexts;

    public override void Interact(Character character) {
        //Dialog.Show("Hola!");
        GetComponent<Character>().photonView.RPC("PlayState", Photon.Pun.RpcTarget.All, "Talking", 0.4f);
        character.transform.rotation = Quaternion.LookRotation(transform.position - character.transform.position, Vector3.up);
        transform.rotation = Quaternion.LookRotation(character.transform.position - transform.position, Vector3.up);
        Dialog.ShowConversation(conversationTexts, transform);
    }

}
