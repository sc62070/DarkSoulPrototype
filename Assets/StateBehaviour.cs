using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;

public class StateBehaviour : StateMachineBehaviour {

    public string identifier = "";

    [Header("Behaviour")]
    public bool useRootMotion = false;

    public bool isBusy = false;
    public bool isEvade = false;
    public bool isAttack = false;
    public bool isEquipped = false;
    public bool isPhysicsEnabled = true;

    [Header("Visual")]
    public EquipmentAccomodation equipmentAccomodation = EquipmentAccomodation.Kubold;

    Animator animator = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = animator;

        animator.GetComponent<Character>().isBusy = isBusy;
        animator.GetComponent<Character>().isEvading = isEvade;
        animator.GetComponent<Character>().isAttacking = isAttack;
        animator.GetComponent<Character>().isEquipped = isEquipped;
        animator.GetComponent<Character>().isPhysicsEnabled = isPhysicsEnabled;

        animator.GetComponent<Character>().Reaccomodate(equipmentAccomodation);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = null;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (useRootMotion) {
            //animator.deltaPosition;
            animator.ApplyBuiltinRootMotion();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

        /*animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, animator.GetComponent<Character>().isBlocking ? 1.0f : 0.0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, animator.GetComponent<Character>().isBlocking ? 1.0f : 0.0f);

        Transform t = animator.transform;

        animator.SetIKRotation(AvatarIKGoal.LeftHand, t.rotation);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, t.position + t.up * 1.2f + t.forward * 0.3f);*/
    }

}

public enum EquipmentAccomodation {
    Kubold, Frank, Own
}
