using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitListener : MonoBehaviour {

    new Collider collider;

    // Start is called before the first frame update
    void Start() {
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.transform.root != transform.root && other.transform.root.GetComponent<Character>() != null) {
            SendMessageUpwards("OnWeaponHit", other.transform.root.GetComponent<Character>());
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.transform.root != transform.root && other.transform.root.GetComponent<Character>() != null) {
            SendMessageUpwards("OnWeaponHit", other.transform.root.GetComponent<Character>());
        }
    }

}
