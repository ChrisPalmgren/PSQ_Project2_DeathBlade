using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecterRange_Attack : Range_Attack
{
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
        
        dt_temp += Time.deltaTime;
        float distanceFromPlayer = Vector3.Distance( Enemy.transform.position, Player.transform.position );
        if ( dt_temp > EnemyBase.AttackRate && distanceFromPlayer < EnemyBase.EnemyDistance )
        {
            dt_temp = 0.0f;
            animator.SetTrigger( "Dash" );
            ShootPlayer();
        }

        if ( gun != null )
        {
            gun.transform.LookAt( Player.transform );
            gun.transform.eulerAngles = new Vector3( gun.transform.eulerAngles.x - 5.0f, gun.transform.eulerAngles.y, gun.transform.eulerAngles.z );
        }

        Vector3 enemyToPlayer = Player.transform.position - Enemy.transform.position;
        Ray ray = new Ray(Enemy.transform.position, enemyToPlayer);
        RaycastHit hit;
        Physics.Raycast( ray, out hit );

        Enemy.transform.rotation = Quaternion.Slerp( Enemy.transform.rotation, Quaternion.LookRotation( Player.transform.position - Enemy.transform.position ), 2f * Time.deltaTime );
        if ( distanceFromPlayer > EnemyBase.EnemyDistance || hit.transform.position != Player.transform.position )
        {
            animator.SetBool( "isChasing", true );
            animator.SetBool( "isAttacking", false );
            return;
        }


    }

    public override void DoDamage()
    {
        base.DoDamage();
    }
}
