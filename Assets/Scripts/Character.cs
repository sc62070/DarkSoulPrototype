using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unimotion;

public class Character : MonoBehaviour {

    public float health = 100f;
    public float maxHealth = 100f;

    public float attackRadius = 0.5f;

    public bool isBlocking = false;

    public AttackMove lastAttackMove = null;

    public HashSet<Object> blockers = new HashSet<Object>();

    public Dictionary<string, AttackMove> attackDictionary = new Dictionary<string, AttackMove>();

    CharacterMotor motor;
    CapsuleCollider capsule;

    // Start is called before the first frame update
    void Awake() {
        motor = GetComponent<CharacterMotor>();
        capsule = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update() {

        motor.canWalk = blockers.Count == 0;
        motor.canJump = blockers.Count == 0;
        motor.canTurn = blockers.Count == 0;

    }

    public void Attack(AttackType type) {
        if(lastAttackMove != null && lastAttackMove.nextAttack != null) {
            motor.animator.CrossFadeInFixedTime(lastAttackMove.nextAttack.stateName, 0.2f);
            lastAttackMove = lastAttackMove.nextAttack;
        } else {
            motor.animator.CrossFadeInFixedTime(AttackMoves.slashR.stateName, 0.2f);
            lastAttackMove = AttackMoves.slashR;
        }
    }

    public void Stagger() {
        motor.animator.CrossFadeInFixedTime("Stagger", 0.2f);
    }

    public void Damage(float q) {
        if(health > 0f) {
            health -= q;
            if (health <= 0f) {
                Kill();
            } else {
                Stagger();
            }
        }
    }

    public void Kill() {
        motor.animator.CrossFadeInFixedTime("Die", 0.2f);
        Destroy(GetComponent<Target>());
    }

    public bool IsBlocked() {
        return blockers.Count > 0;
    }

    public void SendEvent(string evt) {
        if (evt.Equals("strike")) {
            Collider[] cols = Physics.OverlapSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius, LayerMask.GetMask(new string[] { "Characters" }), QueryTriggerInteraction.Ignore);
            foreach (Collider c in cols){
                Character character = c.GetComponent<Character>();
                if(character != null && character != this) {
                    character.GetComponent<CharacterMotor>().TurnTowards(transform.position - character.transform.position, CharacterMotor.TurnBehaviour.Instant);
                    character.Damage(35f);
                }
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        Gizmos.DrawSphere(transform.position + transform.forward * attackRadius * 2f + transform.up * 1.3f, attackRadius);
    }

}

public enum AttackType {
    Light, Heavy
}

public class AttackMove {
    public string stateName;
    public float damageMultiplier = 1f;
    public AttackMove nextAttack;
}

public class AttackMoves {
    public static AttackMove slashRL = new AttackMove() {
        stateName = "Slash RL",
        damageMultiplier = 1f,
        nextAttack = null
    };

    public static AttackMove slashR = new AttackMove() {
        stateName = "Slash R",
        damageMultiplier = 1f,
        nextAttack = slashRL
    };
}