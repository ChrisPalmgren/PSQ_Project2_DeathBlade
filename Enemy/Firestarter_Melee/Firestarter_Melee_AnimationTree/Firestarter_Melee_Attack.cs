using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firestarter_Melee_Attack : AgentStateBase
{
    private float dt_temp;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
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

        //Debug.Log( "Attack" );

        if ( Player.health <= 0.0f )
        {
            animator.SetBool( "isIdle", true );
            animator.SetBool( "isAttacking", false );
            return;
        }

        /*
        Vector3 huntTarget = Player.transform.position - Enemy.transform.position;
        Debug.DrawRay( Enemy.transform.position, huntTarget, Color.red );
        Debug.Log( Vector3.Distance( GetMelee().transform.position, GetPlayer().transform.position ).ToString() );
        */

        Vector3 enemyToPlayer = Player.transform.position - Enemy.transform.position;
        Ray ray = new Ray(Enemy.transform.position, enemyToPlayer);
        RaycastHit hit;
        Physics.Raycast( ray, out hit );

        Enemy.transform.rotation = Quaternion.Slerp( Enemy.transform.rotation, Quaternion.LookRotation( Player.transform.position - Enemy.transform.position ), 2f * Time.deltaTime );
        if ( Vector3.Distance( Enemy.transform.position, Player.transform.position ) > EnemyBase.EnemyDistance || hit.transform.position != Player.transform.position )
         {
            animator.SetBool( "isChasing", true );
            animator.SetBool( "isAttacking", false );
            return;
         }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public void DoDamage()
    {
        // MERGMERGMERG
        Player.TakeDamage( EnemyBase.damage, true );
    }
}
