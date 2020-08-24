using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range_Attack : AgentStateBase
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

        if ( EnemyBase.finalform == true && EnemyBase.isRanged == true )
        {
            EnemyBase.anticipationSound.start();
            ChargeUp.enabled = true;
        }

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

        myRay.origin = Player.transform.position;
        myRay.direction = Enemy.transform.position - Player.transform.position;

        EnemyRay.origin = new Vector3( Enemy.transform.position.x, 1.0f, Enemy.transform.position.z );
        EnemyRay.direction = myRay.GetPoint( EnemyBase.EnemyDistance ) - Enemy.transform.position;

        if ( isDead == true )
            return;

        if ( EnemyBase.health <= 0.0f )
        {
            if ( EnemyBase.finalform == true )
                ChargeUp.enabled = false;
            KillEnemy( animator );
            isDead = true;
            return;
        }

        if ( KillPlayer() == false )
        {
            if ( EnemyBase.finalform == true )
                ChargeUp.enabled = false;
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

        dt_temp += Time.deltaTime;
        if ( EnemyBase.shoot == false )
        {
            hasShot = false;
            if ( SeePlayer() == false )
            {
                if ( EnemyBase.finalform == true )
                    ChargeUp.enabled = false;

                animator.SetBool( "isChasing", true );
                animator.SetBool( "isAttacking", false );
                dt_temp = 0.0f;
                return;
            }
        }
        else if ( EnemyBase.shoot == true )
        {
            ShootPlayer();
            if ( EnemyBase.finalform == true )
            {
                ChargeUp.enabled = false;
                ChargeUp.enabled = true;
            }
            hasShot = true;
            EnemyBase.shoot = false;
        }

        if ( gun != null )
        {
            gun.transform.LookAt( Player.transform );
            gun.transform.eulerAngles = new Vector3( gun.transform.eulerAngles.x - 5.0f, gun.transform.eulerAngles.y, gun.transform.eulerAngles.z );
        }

        Enemy.transform.rotation = Quaternion.Slerp( Enemy.transform.rotation, Quaternion.LookRotation( Player.transform.position - Enemy.transform.position ), 8.0f * Time.deltaTime );

        myRay.origin = Player.transform.position;
        myRay.direction = Enemy.transform.position - Player.transform.position;

        EnemyRay.origin = new Vector3( Enemy.transform.position.x, 1.0f, Enemy.transform.position.z );
        EnemyRay.direction = myRay.GetPoint( EnemyBase.EnemyDistance ) - Enemy.transform.position;

        RaycastHit hit;
        if ( Physics.Raycast( EnemyRay, out hit, 5.0f ) == true )
        {
            if ( hit.transform.gameObject.CompareTag( "Wall" ) == true || hit.transform.gameObject.CompareTag( "ExtraTagForEnemies" ) == true )
            {
                return;
            }
        }

        if ( hasShot == true )
        {
            if ( SeePlayer() == false || Vector3.Distance(Player.transform.position, Enemy.transform.position) > EnemyBase.EnemyDistance + 1.0f
            || Vector3.Distance( Player.transform.position, Enemy.transform.position ) < EnemyBase.EnemyDistance - 1.0f )
            {
                if ( EnemyBase.finalform == true )
                    ChargeUp.enabled = false;

                    animator.SetBool( "isChasing", true );
                    animator.SetBool( "isAttacking", false );
                    return;
            }
            else return;
        }
}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public virtual void DoDamage()
    {
        Player.TakeDamage( EnemyBase.damage, false );
    }
}
