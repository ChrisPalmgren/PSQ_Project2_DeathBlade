using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range_Chase : AgentStateBase
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

        if ( SeePlayer() == false || Vector3.Distance( Enemy.transform.position, Player.transform.position ) > EnemyBase.EnemyDistance + 1.0f  )
        {
            EnemyAgent.isStopped = false;
            EnemyAgent.destination = Player.transform.position;
            EnemyAgent.speed = EnemyBase.runspeed;
            return;
        }
        if ( Vector3.Distance( Enemy.transform.position, Player.transform.position ) > EnemyBase.EnemyDistance - 1.0f &&
             Vector3.Distance( Enemy.transform.position, Player.transform.position ) < EnemyBase.EnemyDistance + 1.0f )
        {
            EnemyAgent.speed = 3.0f;
            EnemyAgent.isStopped = true;
            animator.SetBool( "isChasing", false );
            animator.SetBool( "isAttacking", true );
            return;
        }

        if ( Vector3.Distance( Enemy.transform.position, Player.transform.position ) < EnemyBase.EnemyDistance + 1.0f )
        {
            myRay.origin = Player.transform.position;
            myRay.direction = Enemy.transform.position - Player.transform.position;

            EnemyRay.origin = new Vector3( Enemy.transform.position.x, 1.0f, Enemy.transform.position.z );
            EnemyRay.direction = myRay.GetPoint( EnemyBase.EnemyDistance ) - Enemy.transform.position;

            if ( Vector3.Distance( Enemy.transform.position, Player.transform.position ) < EnemyBase.EnemyDistance - 1.0f && SeePlayer() == true )
            {
                RaycastHit hit;
                if ( Physics.Raycast( EnemyRay, out hit, 5.0f ) == true )
                {
                    if ( hit.transform.gameObject.CompareTag( "Wall" ) == true || hit.transform.gameObject.CompareTag( "ExtraTagForEnemies" ) == true )
                    {
                        EnemyAgent.speed = 3.0f;
                        EnemyAgent.isStopped = true;
                        animator.SetBool( "isChasing", false );
                        animator.SetBool( "isAttacking", true );
                        return;
                    }
                }

                EnemyAgent.SetDestination( myRay.GetPoint( EnemyBase.EnemyDistance ) );
                EnemyAgent.speed = EnemyBase.runspeed;
                EnemyAgent.angularSpeed = 720.0f;
                EnemyAgent.isStopped = false;
            }
        }
    }

     //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
