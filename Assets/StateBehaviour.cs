using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;

public class StateBehaviour : StateMachineBehaviour {

    public bool useRootMotion = false;

    public bool isBusy = false;
    public bool isEvade = false;
    public bool isAttack = false;
    public bool isEquipped = false;

    Animator animator = null;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = animator;

        animator.GetComponent<Character>().isBusy = isBusy;
        animator.GetComponent<Character>().isEvading = isEvade;
        animator.GetComponent<Character>().isAttacking = isAttack;
        animator.GetComponent<Character>().isEquipped = isEquipped;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = null;

        if (isEvade) {
            animator.GetComponent<Character>().isEvading = false;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (useRootMotion) {
            animator.ApplyBuiltinRootMotion();
        }
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
