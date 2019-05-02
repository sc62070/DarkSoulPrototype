using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {

    public static Interactable active;

    public abstract string Description { get; set; }

    private void Awake() {
    }

    public abstract void Interact(Character character);

    /*public void SelectNext() {
        selected = Mathf.Clamp(selected + 1, 0, Actions.Length - 1);
    }

    public void SelectPrevious() {
        selected = Mathf.Clamp(selected - 1, 0, Actions.Length - 1);
    }*/

    protected void Update() {
        Collider[] cols = Physics.OverlapSphere(transform.position, 1f, LayerMask.GetMask(new string[] { "Characters" }));
        bool detected = false;
        foreach (Collider col in cols) {
            Unimotion.Player player = col.GetComponent<Unimotion.Player>();
            if (player != null && player.photonView.IsMine) {
                detected = true;
                active = this;
                break;
            }
        }

        if (!detected && active == this) {
            active = null;
        }
    }

}
