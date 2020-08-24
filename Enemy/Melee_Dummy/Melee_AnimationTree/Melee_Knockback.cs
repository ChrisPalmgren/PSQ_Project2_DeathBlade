using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee_Knockback : AgentStateBase
{
    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );

        EnemyAgent.speed = 3.0f;
        EnemyAgent.isStopped = true;
    }

    public override void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        switch (LastState)
        {
            case 0:
                animator.SetBool( "isKnocked", false );
                animator.SetBool( "isChasing", true );
                break;
            case 1:
                animator.SetBool( "isKnocked", false );
                animator.SetBool( "isAttacking", true );
                break;
        }
    }

    public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        
    }
}
