using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterMelee_Chase : Melee_Chase
{
    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        Firestarter firestarter = EnemyBase as Firestarter;
        firestarter.isFirestarterAttacking = true;

        firestarter.anticipationSound.start();
        firestarter.anticipationSound2.start();
    }

    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
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
}
