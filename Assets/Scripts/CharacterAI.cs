using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour {

    private Character character;
    private Target target;

    void Awake() {
        character = GetComponent<Character>();
    }

    void Start() {
        
    }

    void Update() {

        // Check for enemies
        if(target == null) {
            Collider[] cols = Physics.OverlapSphere(transform.position, 6f, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);

            foreach (Collider c in cols){
                Target t = c.GetComponent<Target>();
                if (t != null && t.gameObject != character.gameObject) {
                    target = t;
                }
            }
        }

        if(target != null) {
            character.Block(true);
        }
        
    }

}
