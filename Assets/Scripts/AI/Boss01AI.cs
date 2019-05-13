using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01AI : BossAI {
    public override AttackMove PickAttack() {
        AttackMove[] moves = {
            new AttackMove("Combo B", 1f, AttackType.Heavy, AttackForce.Huge),
            new AttackMove("Special 1", 1f, AttackType.Heavy, AttackForce.Huge)
        };
        return moves[Random.Range(0, moves.Length)];
    }
}
