using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firestarter_Range_Idle : AgentStateBase
{
    private float dt_temp;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isAttacking", false );
        animator.SetBool( "isDead", false );
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( KillPlayer() == false )
            return;

        if ( EnemyBase.health <= 0.0f )
        {
            KillEnemy( animator );
            return;
        }

        /*
        Debug.Log( "Idle" );
        Vector3 huntTarget = GetPlayer().transform.position - GetMelee().transform.position;
        Debug.DrawRay( GetMelee().transform.position, huntTarget, Color.green );
        Debug.DrawRay( GetMelee().transform.position, GetMelee().transform.forward * 10.0f );
        */

        dt_temp += Time.deltaTime;
        if ( dt_temp >= EnemyBase.InRoomTime )
        {
            animator.SetBool( "isAlerting", true );
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
