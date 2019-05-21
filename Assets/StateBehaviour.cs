using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;
using UnityEngine.Animations;

public class StateBehaviour : StateMachineBehaviour {
    public static int currentStateHash = 0;

    [Header("Behaviour")]
    public bool useRootMotion = false;

    public bool isBusy = false;
    public bool isEvade = false;
    public bool isAttack = false;
    public bool isEquipped = false;
    public bool isPhysicsEnabled = true;

    [Tooltip("If this is set to true, when the animator enters this state, Character.movementMultiplier will be set to 0. This means the character will stop instantly")]
    public bool initMultiplierOnEnter = false;

    [Header("Visual")]
    public EquipmentAccomodation equipmentAccomodation = EquipmentAccomodation.Kubold;
    public Vector3 constantMovement;

    Animator animator = null;
    CharacterMotor motor;
    Character character;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = animator;
        this.motor = animator.GetComponent<CharacterMotor>();
        this.character = animator.GetComponent<Character>();

        //Debug.Log(stateInfo.shortNameHash);
        currentStateHash = stateInfo.shortNameHash;

        character.isBusy = isBusy;
        character.isEvading = isEvade;
        character.isAttacking = isAttack;
        character.isEquipped = isEquipped;
        character.isPhysicsEnabled = isPhysicsEnabled;

        character.Reaccomodate(equipmentAccomodation);

        character.movementMultiplier = initMultiplierOnEnter ? 0f : character.movementMultiplier;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = null;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {


        //                  (                       vvvv Maybe deleting this could solve the bug?
        if (useRootMotion/* && stateInfo.shortNameHash == currentStateHash*/) {
                animator.ApplyBuiltinRootMotion();
            //animator.deltaPosition;
            
        }

        /*if (animator.GetNextAnimatorStateInfo(0).shortNameHash == 0 || animator.GetNextAnimatorStateInfo(0).shortNameHash == stateInfo.shortNameHash) {
            motor.Move(animator.transform.rotation * constantMovement * Time.deltaTime);
        }*/
            
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //animator.GetNextAnimatorClipInfo(0)[0].

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);

        /*if(currentState.IsName("Grounded Combat") && animator.IsInTransition(layerIndex)) {
            Debug.Log(animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime);
        }*/

        if (currentState.shortNameHash.Equals(stateInfo.shortNameHash) || animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash.Equals(stateInfo.shortNameHash)) {
            float targetMovementMultiplier = isBusy ? 0f : 1f;
            character.movementMultiplier = Mathf.MoveTowards(character.movementMultiplier, targetMovementMultiplier, 4f * Time.deltaTime);
            Debug.Log(character.movementMultiplier);
        }

        /*if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorClipInfoCount(layerIndex) > 0 && currentState.IsName("Grounded Combat")) {
            Debug.Log(animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip);
            Debug.Log(currentState.shortNameHash);
        }*/

        /*if (animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash == stateInfo.shortNameHash) {
            float targetMovementMultiplier = isBusy ? 0f : 1f;

            if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorClipInfoCount(layerIndex) > 0) {
                AnimatorClipInfo[] clips = animator.GetNextAnimatorClipInfo(layerIndex);
                float weightOfThis = 1f - clips[0].weight;
                character.movementMultiplier = targetMovementMultiplier * weightOfThis;

            } else {
                character.movementMultiplier = targetMovementMultiplier;
            }
        }*/
        
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
