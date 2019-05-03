using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;
using UnityEngine.AI;

public class HumanAI : CharacterAI {

    public override AttackMove PickAttack() {
        return AttackMoves.slashR;
    }
}

public enum AIState {
    Idle,
    Patroling,
    RunningAway,
    ChasingTarget,
    RoundingTarget,
    AwaitingTargetMove,
    Attacking
}