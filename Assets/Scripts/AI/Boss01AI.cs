using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss01AI : BossAI {
    public override AttackMove PickAttack() {
        return new AttackMove("Combo B", 1f);
    }
}
