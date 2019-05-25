using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;
using UnityEngine.Animations;

public class StateBehaviour : StateMachineBehaviour {
    public static int currentStateHash = 0;

    [Header("Behaviour")]
    public bool useRootMotion = false;

    [Range(0f, 1f)]
    public float targetMovementMultiplier = 1f;
    public bool isEvade = false;
    public bool isAttack = false;
    public bool isEquipped = false;
    public bool isPhysicsEnabled = true;

    [Tooltip("If this is set to true, when the animator enters this state, Character.movementMultiplier will be set to 0. This means the character will stop instantly")]
    public bool initMultiplierOnEnter = false;

    [Header("Other")]
    public bool smoothMovementMultiplier = true;

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

        character.isEvading = isEvade;
        character.isAttacking = isAttack;
        character.isEquipped = isEquipped;
        character.isPhysicsEnabled = isPhysicsEnabled;

        character.Reaccomodate(equipmentAccomodation);

        //character.movementMultiplier = initMultiplierOnEnter ? 0f : character.movementMultiplier;

        startingtMovementMultiplier = character.movementMultiplier;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.animator = null;
    }

    float constantMovementMultiplier = 0f;

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {


        //                  (                       vvvv Maybe deleting this could solve the bug?
        if (useRootMotion/* && stateInfo.shortNameHash == currentStateHash*/) {
            animator.ApplyBuiltinRootMotion();
            //animator.deltaPosition;

        }

        

        float stateWeight = GetWeight(animator, stateInfo, layerIndex);

        // Apply constant movement

        motor.Move(animator.transform.rotation * constantMovement * stateWeight * Time.deltaTime);
        
        

        //motor.Move(animator.transform.rotation * constantMovement * (1f - character.movementMultiplier) * Time.deltaTime);

    }

    private float stateWeight = 0f;
    private float startingtMovementMultiplier = 999f;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //animator.GetNextAnimatorClipInfo(0)[0].

        // Useful code to understand states and clips
        /*Debug.Log("====================================================================================");
        Debug.Log("");
        Debug.Log("CURRENT STATE = " + animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash);
        Debug.Log("CURRENT CLIPS");
        foreach (AnimatorClipInfo aci in animator.GetCurrentAnimatorClipInfo(layerIndex)) {
            Debug.Log(aci.clip + ", weight: " + aci.weight);
        }
        Debug.Log("");
        Debug.Log("NEXT STATE = " + animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash);
        Debug.Log("NEXT CLIPS");
        foreach (AnimatorClipInfo aci in animator.GetNextAnimatorClipInfo(layerIndex)) {
            Debug.Log(aci.clip + ", weight: " + aci.weight);
        }*/
        //float stateWeight = GetWeight(animator, stateInfo, layerIndex);

        // If the current state is this one...
        bool currentIsThis = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash.Equals(stateInfo.shortNameHash);
        bool nextIsThis = animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash.Equals(stateInfo.shortNameHash);
        bool inTransition = animator.IsInTransition(layerIndex);

        bool inControl = (currentIsThis && !inTransition) || nextIsThis;

        // This code could be simplified now that I have the method GetWeight()
        if (inControl && inTransition) {
            // If it enters here, it means we are in control because we are the NEXT state
            AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layerIndex);
            character.movementMultiplier = Mathf.Lerp(startingtMovementMultiplier, targetMovementMultiplier, transition.normalizedTime);
            stateWeight = transition.normalizedTime;
        } else if (inControl) {
            // If it enters here, it means we are in control because we are the CURRENT state and there is NO NEXT state
            character.movementMultiplier = targetMovementMultiplier;
            stateWeight = 1f;
        } else {
            stateWeight = 0f;
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

    public float GetWeight(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float stateWeight = 0f;
        AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(layerIndex);

        bool currentIsThis = animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash.Equals(stateInfo.shortNameHash);
        bool nextIsThis = animator.GetNextAnimatorStateInfo(layerIndex).shortNameHash.Equals(stateInfo.shortNameHash);
        bool inTransition = animator.IsInTransition(layerIndex);

        bool inControl = (currentIsThis && !inTransition) || nextIsThis;

        if (currentIsThis && !inTransition) {
            stateWeight = 1f;
        } else if (currentIsThis && inTransition) {
            stateWeight = 1f - transition.normalizedTime;
        } else if (nextIsThis && inTransition) {
            stateWeight = transition.normalizedTime;
        } else {
            stateWeight = 0f;
        }
        return stateWeight;
    }

}

public enum EquipmentAccomodation {
    Kubold, Frank, Own
}
