using System.Collections;
using System.Collections.Generic;
using Game.Enemy;
using UnityEngine;

public class HealOverBehaviour : StateMachineBehaviour
{
    float _lerpCount = 0;
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
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<RangedEnemyBehaviour>().ReadyToHeal = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,Mathf.Lerp(_lerpCount,1,Time.deltaTime));

        _lerpCount += Time.deltaTime;
        if (_lerpCount >= 1)
            _lerpCount = 1;
    }
}
