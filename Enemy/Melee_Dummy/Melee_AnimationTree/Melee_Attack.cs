using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Melee_Attack : AgentStateBase
{
    protected float dt_temp;
    protected Animator animator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isDead", false );
        this.animator = animator;

        this.animator.SetFloat("AttackSpeed", 0.5f);
        EnemyBase.anticipationSound.start();

        LastState = 1;

        EnemyAgent.speed = 3.0f;
        EnemyAgent.isStopped = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ( EnemyAgent.enabled != false )
        {
            EnemyAgent.speed = 3.0f;
            EnemyAgent.isStopped = true;
        }

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
            animator.SetBool( "isIdle", true );
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

        float distanceFromPlayer = Vector3.Distance( Enemy.transform.position, Player.transform.position );
        dt_temp += Time.deltaTime;
        if ( EnemyBase.finalform == false )
        {
            if ( dt_temp > EnemyBase.AttackRate && distanceFromPlayer < EnemyBase.EnemyDistance )
            {
                dt_temp = 0.0f;
                EnemyBase.attackSound.start();
                DoDamage();
            }
        }

        Vector3 enemyToPlayer = Player.transform.position - Enemy.transform.position;
        Ray ray = new Ray(Enemy.transform.position, enemyToPlayer);
        RaycastHit hit;
        Physics.Raycast( ray, out hit );

        Enemy.transform.rotation = Quaternion.Slerp( Enemy.transform.rotation, Quaternion.LookRotation( Player.transform.position - Enemy.transform.position ), 8.0f * Time.deltaTime );


        if ( distanceFromPlayer > EnemyBase.EnemyDistance || hit.transform.position != Player.transform.position )
         {
            animator.SetBool( "isChasing", true );
            animator.SetBool( "isAttacking", false );
            return;
         }
    }

    public virtual void DoDamage()
    {
        Player.TakeDamage( EnemyBase.damage, true );
    }
}
