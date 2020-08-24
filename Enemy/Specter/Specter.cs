using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Specter : EnemyBase
{
    //////////////////////////////////////////
    // VARIABLES
    //////////////////////////////////////////

    [SerializeField]
    float dashSpeed = 1.0f;
    [SerializeField]
    float dashLength = 1.0f;

    //////////////////////////////////////////

    bool dashing = false;
    Vector3 dashingDirection = new Vector3();
    Quaternion startRot = new Quaternion();

    private FMOD.Studio.EventInstance dashSound1;
    private FMOD.Studio.EventInstance dashSound2;

    ////////////////////////////////////////////////////////////

    protected override void AttachSound()
    {
        ////////////////////////////////////////////////////////////
        // AUDIO
        ////////////////////////////////////////////////////////////

        Rigidbody rb = GetComponent<Rigidbody>();

        hitSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_hit" );
        RuntimeManager.AttachInstanceToGameObject( hitSound, transform, rb );
        deadSound = RuntimeManager.CreateInstance( "event:/Specter/specter_death" );
        RuntimeManager.AttachInstanceToGameObject( deadSound, transform, rb );
        disappearsSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_disappears" );
        RuntimeManager.AttachInstanceToGameObject( disappearsSound, transform, rb );

        dashSound1 = RuntimeManager.CreateInstance( "event:/Specter/specter_dash_1" );
        RuntimeManager.AttachInstanceToGameObject( dashSound1, transform, rb );
        dashSound2 = RuntimeManager.CreateInstance( "event:/Specter/specter_dash_2" );
        RuntimeManager.AttachInstanceToGameObject( dashSound2, transform, rb );

        if ( isRanged == true )
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Grunt/ranged_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Grunt/ranged_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }
        else
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Specter/specter_melee_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Specter/specter_melee_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }

        ////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////

    protected override void Update()
    {
        base.Update();

        ////////////////////////////////////////////////////////////
        // IF DASHING, ADD TO POSITION
        ////////////////////////////////////////////////////////////

        if ( dashing == true )
        {
            transform.rotation = startRot;
            transform.position += dashingDirection * Time.deltaTime * dashSpeed;
        }

        ////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////
    // STOP DASH IF ENEMY HITS WALL
    ////////////////////////////////////////////////////////////

    private void OnCollisionEnter( Collision collision )
    {
        if ( collision.collider.CompareTag( "Wall" ) == true && dashing == true )
            EndDash();
    }
    ////////////////////////////////////////////////////////////

    public void SetDash( bool state )
    {
        dashing = state;

        if ( dashing == true )
        {
            dashSound1.start();
            dashSound2.start();

            ////////////////////////////////////////////////////////////
            // SET ROTATION TO BE USED AND WHAT DIRECTION TO DASH
            ////////////////////////////////////////////////////////////

            animator.ResetTrigger( "EndDash" );
            StartCoroutine( DoDash() );
            startRot = transform.rotation;
            if ( isRanged == false )
                dashingDirection = -transform.forward;
            else 
            {
                int randomNumber = Random.Range( 0, 100 );
                if( randomNumber > 50 )
                    dashingDirection = transform.right;
                else
                    dashingDirection = -transform.right;
            }
           
            ////////////////////////////////////////////////////////////
        }
    }
    
    ////////////////////////////////////////////////////////////

    void EndDash()
    {
        dashing = false;
        animator.ResetTrigger( "Dash" );
        animator.SetTrigger( "EndDash" );
    }
    
    ////////////////////////////////////////////////////////////

    IEnumerator DoDash()
    {
        yield return new WaitForSeconds( dashLength );
        EndDash();
    }

    ////////////////////////////////////////////////////////////
}
