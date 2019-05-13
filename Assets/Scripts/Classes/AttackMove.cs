using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackMove {
    public string stateName;

    public AttackType type = AttackType.Light;
    public float damageMultiplier;
    public AttackMove nextAttack;
    public AttackForce force;

    public AttackMove(string stateName, float damageMultiplier) : this(stateName, damageMultiplier, AttackType.Light) {
    }

    public AttackMove(string stateName, float damageMultiplier, AttackType type) : this(stateName, damageMultiplier, type, AttackForce.Low) {
    }

    public AttackMove(string stateName, float damageMultiplier, AttackType type, AttackForce force) {
        this.stateName = stateName;
        this.type = type;
        this.damageMultiplier = damageMultiplier;
        this.force = force;
    }

    
}

public enum AttackType {
    Light, Heavy
}

// Determine what effect the attack will have on the victim
public enum AttackForce {
    None, Low, High, Huge, Crushing
}

public class AttackMoves {

    // Heavy slashes
    public static AttackMove slashHeavy = new AttackMove("Slash Heavy", 2f, AttackType.Heavy) {
        nextAttack = null
    };

    // Light slashes
    public static AttackMove slashRL = new AttackMove("Slash RL", 1f, AttackType.Light) {
        nextAttack = null
    };

    public static AttackMove slashR = new AttackMove("Slash R", 1f, AttackType.Light) {
        nextAttack = slashRL
    };
}
