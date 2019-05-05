using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unimotion;

public abstract class CharacterAI : MonoBehaviour {

    private Character character;
    private CharacterMotor motor;

    public AIState state;

    [Range(0.1f, 1f)]
    public float combatSpeed = 1f;
    [Range(0f, 1f)]
    public float combatAggressiveness = 1f;

    private float roundDirectionChangeMultiplier = 1f;

    private bool shouldBlock = false;

    public void Awake() {
        character = GetComponent<Character>();
        motor = GetComponent<CharacterMotor>();
    }

    public void Start() {
        StartCoroutine(AIThinkingRoutine());
        StartCoroutine(AIDirectionChangeRoutine());
        StartCoroutine(AIBlockRoutine());
        StartCoroutine(AIAttackRoutine());
    }

    void Update() {
        if (character.photonView.IsMine && character.health > 0f) {

            // Check for targets
            if (character.target == null) {
                Collider[] cols = Physics.OverlapSphere(transform.position, 10f, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);

                foreach (Collider c in cols) {
                    Target t = c.GetComponent<Target>();
                    if (t != null && t.gameObject != character.gameObject) {
                        character.target = t;
                    }
                }
            }

            bool canReach = false;
            NavMeshPath path = new NavMeshPath();

            if (character.target != null) {
                //motor.TurnTowards((character.target.transform.position - transform.position));

                character.Block(shouldBlock);

                // Calculate path to target
                path = new NavMeshPath();
                canReach = NavMesh.CalculatePath(transform.position, character.target.transform.position, NavMesh.AllAreas, path);
            }

            // Act according to state
            switch (state) {
                case AIState.Idle:
                    break;
                case AIState.ChasingTarget:
                    if (character.target != null) {
                        if (Vector3.Distance(transform.position, character.target.transform.position) > 2f) {

                            /// Chase target
                            if (canReach && path.corners.Length >= 2) {
                                motor.Walk((path.corners[1] - transform.position).normalized);
                            }
                        } else {
                            character.Attack(AttackType.Light);
                        }
                    }
                    break;
                case AIState.RoundingTarget:
                    motor.Walk(transform.right * roundDirectionChangeMultiplier * combatSpeed);
                    break;
                case AIState.Attacking:

                    /// Chase target
                    if (canReach && path.corners.Length >= 2) {
                        motor.Walk((path.corners[1] - transform.position).normalized);
                    }

                    if (Vector3.Distance(transform.position, character.target.transform.position) < 2f) {
                        character.Attack(PickAttack());
                        state = AIState.RoundingTarget;
                    }
                    break;
            }
        }

    }

    private IEnumerator AIThinkingRoutine() {
        while (true) {
            yield return new WaitForEndOfFrame();

            // Logic to change states
            switch (state) {
                case AIState.Idle:
                    if (character.target != null) {
                        state = AIState.ChasingTarget;
                    }
                    break;
                case AIState.ChasingTarget:

                    // If close enough to target, start rounding him
                    if (Vector3.Distance(transform.position, character.target.transform.position) <= 4f) {
                        state = AIState.RoundingTarget;
                    }
                    break;
                case AIState.RoundingTarget:
                    /*yield return new WaitForSeconds(Random.Range(2f, 8f));

                    if (character.target != null) {
                        state = AIState.Attacking;
                    } else {
                        state = AIState.Idle;
                    }*/
                    break;
            }
        }
    }

    private IEnumerator AIDirectionChangeRoutine() {
        while (true) {

            yield return new WaitForSeconds(2f);
            float[] values = new float[] { 1f, 0f, -1f };
            roundDirectionChangeMultiplier = values[Random.Range(0, values.Length)];

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator AIAttackRoutine() {
        while (true) {
            if (state == AIState.RoundingTarget) {
                yield return new WaitForSeconds(Random.Range(1f * combatAggressiveness, 4f * combatAggressiveness));

                if (character.target != null) {
                    state = AIState.Attacking;
                } else {
                    state = AIState.Idle;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator AIBlockRoutine() {
        while (true) {
            yield return new WaitForSeconds(1f);

            if (character.target != null && character.stamina / character.maxStamina > 0.8f) {
                shouldBlock = true;
            } else {
                shouldBlock = false;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public abstract AttackMove PickAttack();

}
