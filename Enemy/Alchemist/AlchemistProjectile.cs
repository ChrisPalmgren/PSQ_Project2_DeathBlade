using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AlchemistProjectile : EnemyProjectile
{
    VisualEffect alchemistUI;

    protected override void Start()
    {
        base.Start();

        GameObject alchemistUIObj = GameObject.Find( "HackingScreen VFX" );
        if ( alchemistUIObj != null )
            alchemistUI = alchemistUIObj.GetComponent<VisualEffect>();
    }

    public override void Initialize( EnemyBase enemy )
    {
        base.Initialize( enemy );
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( alchemistUI == null )
            return;

        if ( other.gameObject.CompareTag( "ProjectileColl" ) )
        {
            Player.TakeDamage( Enemy.damage, false );
            alchemistUI.Play();

            DeInitialize();
        }
        else if ( other.gameObject.CompareTag( "Wall" ) )
            DeInitialize();
    }
}
