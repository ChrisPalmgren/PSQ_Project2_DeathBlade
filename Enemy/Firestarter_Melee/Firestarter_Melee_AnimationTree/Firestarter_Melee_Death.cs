﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firestarter_Melee_Death : AgentStateBase
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter( animator, stateInfo, layerIndex );
        animator.SetBool( "isIdle", false );
        animator.SetBool( "isAlerting", false );
        animator.SetBool( "isChasing", false );
        animator.SetBool( "isAttacking", false );

        // TODO: Implement function for health and mana regeneration
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool( "Exit", true );
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Enemy.SetActive( false );
    }
}
