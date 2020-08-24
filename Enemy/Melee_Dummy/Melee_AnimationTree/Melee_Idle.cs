using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Idle : AgentStateBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isAttacking", false );
        animator.SetBool( "isDead", false );

        animator.speed = 1.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( isDead == true )
        {
            return;
        }
            
        if ( EnemyBase.health <= 0.0f )
        {
            KillEnemy( animator );
            isDead = true;
            return;
        }

        if ( KillPlayer() == false )
            return;

        if ( EnemyBase.isHit == true )
        {
            animator.Play( "KnockBack" );
            //animator.SetBool( "isChasing", false );
            //animator.SetBool( "isKnocked", true );
            EnemyBase.isHit = false;
            return;
        }

        if ( EnemyBase.isPlayerDetected == true )
        {
            animator.SetBool( "isChasing", true );
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
