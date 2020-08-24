using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterMelee_Death : Melee_Death
{
    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isAttacking", false );

        Firestarter firestarter = EnemyBase as Firestarter;
        firestarter.DoSplashDamage( firestarter.SplashDamageDeath, firestarter.SplashRangeDeath, firestarter.transform );
    }
}
