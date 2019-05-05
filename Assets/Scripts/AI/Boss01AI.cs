using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01AI : BossAI {
    public override AttackMove PickAttack() {
        AttackMove[] moves = { new AttackMove("Combo B", 1f), new AttackMove("Special 1", 1f)};
        return moves[Random.Range(0, moves.Length)];
    }
}
