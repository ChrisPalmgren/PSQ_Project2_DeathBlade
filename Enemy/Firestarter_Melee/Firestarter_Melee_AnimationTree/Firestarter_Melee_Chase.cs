using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firestarter_Melee_Chase : AgentStateBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        EnemyAgent.angularSpeed = 200.0f;
        EnemyAgent.acceleration = 50.0f;

        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isAttacking", false );
        animator.SetBool( "isDead", false );

    }

     //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( KillPlayer() == false )
        {
            EnemyAgent.isStopped = true;
            animator.SetBool( "isIdle", true );
            animator.SetBool( "isChasing", false );
            return;
        }

        if ( EnemyBase.health <= 0.0f )
        {
            EnemyAgent.isStopped = true;
            KillEnemy( animator );
            return;
        }

        /*
        Debug.Log( "Chase" );
        Vector3 huntTarget = GetPlayer().transform.position - GetMelee().transform.position;
        Debug.DrawRay( GetMelee().transform.position, huntTarget, Color.yellow );
        Debug.DrawRay( GetMelee().transform.position, GetMelee().transform.forward * 10.0f );
        */

        Vector3 enemyToPlayer = Player.transform.position - Enemy.transform.position;
        Ray ray = new Ray(Enemy.transform.position, enemyToPlayer);
        RaycastHit hit;
        Physics.Raycast( ray, out hit );

        EnemyAgent.isStopped = false;
        EnemyAgent.destination = Player.transform.position;
        EnemyAgent.speed = EnemyBase.runspeed;
        //if ( Vector3.Distance( Enemy.transform.position, Player.transform.position ) < EnemyBase.EnemyDistance)
        //{
        //    Player.TakeDamage( EnemyBase.damage );
        //    Enemy.SetActive( false );
        //    return;
        //}
    }

     //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
