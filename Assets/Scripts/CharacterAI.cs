using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour {

    private Character character;
    private CharacterMotor motor;

    void Awake() {
        character = GetComponent<Character>();
        motor = GetComponent<CharacterMotor>();
    }

    void Start() {

    }

    void Update() {

        if (character.health > 0f) {

            // Check for enemies
            if (character.target == null) {
                Collider[] cols = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);

                foreach (Collider c in cols) {
                    Target t = c.GetComponent<Target>();
                    if (t != null && t.gameObject != character.gameObject) {
                        character.target = t;
                    }
                }
            }

            if (character.target != null) {
                character.Block(true);

                motor.TurnTowards((character.target.transform.position - transform.position));

                if (Vector3.Distance(transform.position, character.target.transform.position) > 2f) {
                    NavMeshPath path = new NavMeshPath();
                    bool canReach = NavMesh.CalculatePath(transform.position, character.target.transform.position, NavMesh.AllAreas, path);
                    if (canReach) {
                        motor.Walk((path.corners[1] - transform.position).normalized);
                    }
                } else {
                    character.Attack(AttackType.Light);
                }

            }
        }

    }

}
