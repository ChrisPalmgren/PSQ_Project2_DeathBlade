﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemistMelee_Attack : Melee_Attack
{
    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateUpdate( animator, stateInfo, layerIndex );
    }

    public override void DoDamage()
    {
        base.DoDamage();
        Alchemist alchemist = EnemyBase as Alchemist;
        alchemist.SetAlchemistScreen();
    }
}
