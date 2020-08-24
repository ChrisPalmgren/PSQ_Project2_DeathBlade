using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecterDash : AgentStateBase
{
    private float dt_temp;
    protected Animator animator;
    Specter specter;
    Quaternion startRot;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isDead", false );

        specter = EnemyBase as Specter;
        startRot = specter.transform.rotation;
        specter.SetDash( true );
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( KillPlayer() == false )
        {
            animator.SetBool( "isIdle", true );
            return;
        }

        if ( EnemyBase.health <= 0.0f )
        {
            KillEnemy( animator );
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        specter.SetDash( false );
        animator.SetBool( "isChasing", true );
        animator.ResetTrigger( "Dash" );
    }
}
