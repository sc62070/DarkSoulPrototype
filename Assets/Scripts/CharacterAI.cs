using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unimotion;

public class CharacterAI : MonoBehaviour {

    public Target target;

    private Character character;
    private CharacterMotor motor;

    void Awake() {
        character = GetComponent<Character>();
        motor = GetComponent<CharacterMotor>();
    }

    void Start() {
        
    }

    void Update() {

        // Check for enemies
        if(target == null) {
            Collider[] cols = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);

            foreach (Collider c in cols){
                Target t = c.GetComponent<Target>();
                if (t != null && t.gameObject != character.gameObject) {
                    target = t;
                }
            }
        }

        if(target != null) {
            character.Block(true);

            motor.TurnTowards((target.transform.position - transform.position));

            if (Vector3.Distance(transform.position, target.transform.position) > 2f) {
                NavMeshPath path = new NavMeshPath();
                bool canReach = NavMesh.CalculatePath(transform.position, target.transform.position, NavMesh.AllAreas, path);
                if (canReach) {
                    motor.Walk((path.corners[1] - transform.position).normalized);
                }
            } else {
                character.Attack(AttackType.Light);
            }
            
        }
        
    }

}
