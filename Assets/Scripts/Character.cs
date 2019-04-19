using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unimotion;
using Photon.Pun;

public class Character : MonoBehaviourPun {

    public float health = 100f;
    public float maxHealth = 100f;

    public float stamina = 100f;
    public float maxStamina = 100f;

    public float attackRadius = 0.5f;

    public bool isBlocking = false;
    public bool isEvading = false;

    public AttackMove lastAttackMove = null;
    public float lastAttackTimer = 0f;

    public List<Object> blockers = new List<Object>();

    CharacterMotor motor;
    CapsuleCollider capsule;

    private float timeSinceStaminaConsume = 0f;
    private float timeSinceStaminaRunout = 0f;

    void Awake() {
        motor = GetComponent<CharacterMotor>();
        capsule = GetComponent<CapsuleCollider>();

        motor.OnLand += delegate () {
            photonView.RPC("PlayState", RpcTarget.All, "Land", 0.1f);
        };
    }

    // Update is called once per frame
    void Update() {

        motor.canWalk = blockers.Count == 0;
        motor.canJump = blockers.Count == 0;
        motor.canTurn = blockers.Count == 0;

        if (photonView.IsMine) {
            motor.animator.SetBool("Blocking", isBlocking);

            if (!IsBlocked()) {
                lastAttackTimer -= Time.deltaTime;
            }
        }

    }

    void LateUpdate() {

        timeSinceStaminaConsume += Time.deltaTime;
        timeSinceStaminaRunout += Time.deltaTime;

        // Regenerate stamina
        if (timeSinceStaminaConsume > 1.5f && timeSinceStaminaRunout > 3f && !IsBlocked()) {
            stamina = Mathf.Clamp(stamina + 30f * Time.deltaTime, 0f, maxStamina);
        }

        if (IsBlocked() || isEvading) {
            isBlocking = false;
        }

    }

    public void Attack(AttackType type) {

        if (photonView.IsMine && !IsBlocked() && stamina > 0f) {
            isBlocking = false;
            isEvading = false;

            AttackMove move = type == AttackType.Light ? AttackMoves.slashR : AttackMoves.slashHeavy;

            if (lastAttackMove != null && lastAttackMove.nextAttack != null && lastAttackTimer > 0f) {
                move = lastAttackMove.nextAttack;
            }

            lastAttackTimer = 0.5f;
            lastAttackMove = move;

            photonView.RPC("PlayState", RpcTarget.All, move.stateName, 0.2f);

        }

    }

    [PunRPC]
    public void Stagger() {
        if (photonView.IsMine) {
            photonView.RPC("PlayState", RpcTarget.All, "Stagger", 0.2f);
        }
    }

    public void Evade(Direction direction) {

        isBlocking = false;

        switch (direction) {
            case Direction.Left:
                photonView.RPC("PlayState", RpcTarget.All, "Dodge Left", 0.2f);
                break;
            case Direction.Right:
                photonView.RPC("PlayState", RpcTarget.All, "Dodge Right", 0.2f);
                break;
            case Direction.Up:
                photonView.RPC("PlayState", RpcTarget.All, "Dodge Front", 0.2f);
                break;
            case Direction.Down:
            case Direction.None:
                photonView.RPC("PlayState", RpcTarget.All, "Dodge Back", 0.2f);
                break;
        }
    }

    public void Block(bool flag) {
        bool tmp = isBlocking;
        isBlocking = false;
        if (flag == true && !IsBlocked() && motor.Grounded && !isEvading) {
            isBlocking = true;
        }
    }

    [PunRPC]
    public void Damage(float q, Vector3 direction) {
        if (health > 0f) {
            health -= q;

            motor.TurnTowards(-direction, CharacterMotor.TurnBehaviour.Instant);
            if (health <= 0f) {
                Kill();
            } else {
                Stagger();
            }
        }
    }

    [PunRPC]
    public void AttemptDamage(float q, Vector3 direction, int attackerViewId) {

        Character attacker = PhotonView.Find(attackerViewId).GetComponent<Character>();

        if (isBlocking && Vector3.Angle(transform.forward, attacker.transform.forward) > 90f) {
            attacker.photonView.RPC("Stagger", RpcTarget.All);
        } else {
            Damage(q, direction);
        }
    }

    [PunRPC]
    public void PlayState(string name, float fade) {
        motor.animator.CrossFadeInFixedTime(name, fade);
    }

    public void Kill() {
        motor.animator.CrossFadeInFixedTime("Die", 0.2f);
        Destroy(GetComponent<Target>());
    }

    public bool IsBlocked() {
        return blockers.Count > 0;
    }

    public bool ConsumeStamina(float q) {
        if (stamina > 0f) {
            stamina = Mathf.Clamp(stamina - q, 0f, maxStamina);

            timeSinceStaminaConsume = 0f;

            if (stamina <= 0f) {
                timeSinceStaminaRunout = 0f;
            }

            return true;
        }
        return false;
    }

    public void SendEvent(string evt) {
        if (photonView.IsMine) {
            if (evt.Equals("strike")) {

                // The animation reached the point in which stamina must be consumed
                ConsumeStamina(30f * lastAttackMove.damageMultiplier);

                Collider[] cols = Physics.OverlapSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);
                foreach (Collider c in cols) {
                    Character character = c.GetComponent<Character>();
                    if (character != null && character != this && !character.isEvading) {
                        character.photonView.RPC("AttemptDamage", RpcTarget.All, 35f, character.transform.position - transform.position, photonView.ViewID);
                    }
                }
            } else if (evt.Equals("cancelBlockMovement")) {
                GetComponent<Character>().blockers.RemoveAll(delegate (Object o) {
                    return o.GetType() == typeof(StateBehaviour);
                });
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Gizmos.DrawSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius);
    }

}

public enum AttackType {
    Light, Heavy
}

[System.Serializable]
public class AttackMove {
    public string stateName;
    public AttackType type;
    public float damageMultiplier = 1f;
    public AttackMove nextAttack;
}

public class AttackMoves {

    // Heavy slashes
    public static AttackMove slashHeavy = new AttackMove() {
        stateName = "Slash Heavy",
        type = AttackType.Heavy,
        damageMultiplier = 2f,
        nextAttack = null
    };

    // Light slashes
    public static AttackMove slashRL = new AttackMove() {
        stateName = "Slash RL",
        type = AttackType.Light,
        damageMultiplier = 1f,
        nextAttack = null
    };

    public static AttackMove slashR = new AttackMove() {
        stateName = "Slash R",
        type = AttackType.Light,
        damageMultiplier = 1f,
        nextAttack = slashRL
    };
}