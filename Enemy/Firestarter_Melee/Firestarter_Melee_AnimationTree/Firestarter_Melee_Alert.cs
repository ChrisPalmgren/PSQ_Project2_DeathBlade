using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firestarter_Melee_Alert : AgentStateBase
{
    private float dt_temp;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isAttacking", false );
        animator.SetBool( "isDead", false );
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

        /*
        Debug.Log( "Alert" );
        Vector3 huntTarget = GetPlayer().transform.position - GetMelee().transform.position;
        Debug.DrawRay( GetMelee().transform.position, huntTarget, Color.yellow );
        Debug.DrawRay( GetMelee().transform.position, GetMelee().transform.forward * 10.0f );
        */

        //Enemy.transform.forward = Player.transform.position - Enemy.transform.position;
        Enemy.transform.rotation = Quaternion.Slerp( Enemy.transform.rotation, Quaternion.LookRotation( Player.transform.position - Enemy.transform.position ), 2f * Time.deltaTime );
        dt_temp += Time.deltaTime;
        if ( dt_temp >= EnemyBase.AlertTime )
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
