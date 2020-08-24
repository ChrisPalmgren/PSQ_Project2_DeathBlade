using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range_Death : AgentStateBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isAttacking", false );

        // TODO: Implement function for health and mana regeneration

        isAnimationFinished = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemyAgent.isStopped = true;
        if ( isAnimationFinished == true )
        {
            OnStateExit( animator, stateInfo, layerIndex );
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Destroy( animator.gameObject );
        animator.transform.gameObject.SetActive( false );
    }
}
