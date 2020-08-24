using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterRange_Chase : Range_Chase
{
    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        Firestarter firestarter = EnemyBase as Firestarter;
        firestarter.isFirestarterAttacking = true;
    }
}
