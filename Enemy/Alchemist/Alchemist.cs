using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using FMODUnity;

public class Alchemist : EnemyBase
{
    //////////////////////////////////////////
    // VARIABLES
    //////////////////////////////////////////

    [SerializeField]
    float alchemistImageCooldown = 1.0f;
    VisualEffect alchemistUI;
    
    [HideInInspector] public FMOD.Studio.EventInstance glitchSound;

    //////////////////////////////////////////

    protected override void Start()
    {
        base.Start();
        alchemistUI = GameObject.Find( "HackingScreen VFX" ).GetComponent<VisualEffect>();
        enemyBullets = GameObject.Find( "Alchemist Bullets" ).GetComponent<VFXList>();
    }

    ////////////////////////////////////////////////////////////

    protected override void AttachSound()
    {
        ////////////////////////////////////////////////////////////
        // AUDIO
        ////////////////////////////////////////////////////////////

        Rigidbody rb = GetComponent<Rigidbody>();

        hitSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_hit" );
        RuntimeManager.AttachInstanceToGameObject( hitSound, transform, rb );
        deadSound = RuntimeManager.CreateInstance( "event:/Alchemist - Hacker/alchemit_death" );
        RuntimeManager.AttachInstanceToGameObject( deadSound, transform, rb );
        disappearsSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_disappears" );
        RuntimeManager.AttachInstanceToGameObject( disappearsSound, transform, rb );

        //glitchSound = RuntimeManager.CreateInstance( "event:/Alchemist - Hacker/alchemist_glitch" );
        //RuntimeManager.AttachInstanceToGameObject( glitchSound, transform, rb );

        if ( isRanged == true )
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Alchemist - Hacker/alchemist_ranged_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Alchemist - Hacker/alchemist_ranged_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }
        else
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Alchemist - Hacker/alchemist_melee_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Alchemist - Hacker/alchemist_melee_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }

        ////////////////////////////////////////////////////////////
    }

    //////////////////////////////////////////

    public void SetAlchemistScreen()
    {
        RuntimeManager.PlayOneShotAttached( "event:/Alchemist - Hacker/alchemist_glitch", gameObject );
        alchemistUI.Play();
        //Debug.Log( "playing" );
    }
    
    //////////////////////////////////////////

    protected override IEnumerator FlashColor()
    {
        for ( int i = 0; i < 4; ++i )
        {
            GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds( 0.1f );
            GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds( 0.1f );
        }
    }
    
    //////////////////////////////////////////
}
