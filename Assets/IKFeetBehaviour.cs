using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;

public class IKFeetBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 slopeNormal = animator.GetComponent<PlayerMotor>().SlopeNormal;
        if (slopeNormal != Vector3.zero)
        {

        Quaternion rot = animator.GetComponent<PlayerMotor>().GetPlayerRotation();
        Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, slopeNormal) * rot;
        animator.SetIKRotation(AvatarIKGoal.LeftFoot,targetRot);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot,1);
        animator.SetIKRotation(AvatarIKGoal.RightFoot,targetRot);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
        }
        else
        {
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
}
