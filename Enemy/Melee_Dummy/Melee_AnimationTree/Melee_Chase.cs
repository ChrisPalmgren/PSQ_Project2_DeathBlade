using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Chase : AgentStateBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        EnemyAgent.angularSpeed = 200.0f;
        EnemyAgent.acceleration = 50.0f;

        LastState = 0;

        animator.SetBool( "isKnocked", false );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isAttacking", false );
        animator.SetBool( "isDead", false );

    }

     //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( isDead == true )
            return;

        if ( EnemyBase.health <= 0.0f )
        {
            KillEnemy( animator );
            isDead = true;
            return;
        }

        if ( KillPlayer() == false )
        {
            EnemyAgent.isStopped = true;
            animator.SetBool( "isIdle", true );
            animator.SetBool( "isChasing", false );
            return;
        }

        if ( EnemyBase.isHit == true )
        {
            animator.Play( "KnockBack" );
            //animator.SetBool( "isChasing", false );
            //animator.SetBool( "isKnocked", true );
            EnemyBase.isHit = false;
            return;
        }

        Vector3 enemyToPlayer = Player.transform.position - Enemy.transform.position;
        Ray ray = new Ray(Enemy.transform.position, enemyToPlayer);
        RaycastHit hit;
        Physics.Raycast( ray, out hit );

        EnemyAgent.isStopped = false;
        EnemyAgent.destination = Player.transform.position;
        EnemyAgent.speed = EnemyBase.runspeed;
        if ( Vector3.Distance( Enemy.transform.position, Player.transform.position ) < EnemyBase.EnemyDistance )
        {
            EnemyAgent.speed = 3.0f;
            EnemyAgent.isStopped = true;
            animator.SetBool( "isAttacking", true );
            return;
        }
    }

     //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
