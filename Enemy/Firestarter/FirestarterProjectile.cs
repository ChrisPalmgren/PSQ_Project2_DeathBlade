using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirestarterProjectile : EnemyProjectile
{
    //private GameObject[] otherEnemies;
    LayerMask EnemyLayer = 0;
    //private bool isProjectileConnected = false;
    private Firestarter firestarter = null;

    LevelManager levelManager = null;

    float splashdamage = 0.0f;
    float splashrange = 0.0f;

    protected override void Start()
    {
        base.Start();
    }

    public override void Initialize( EnemyBase enemy )
    {
        base.Initialize( enemy );
        EnemyLayer = ( 1 << LayerMask.NameToLayer( "Enemy" ) );
        firestarter = enemy as Firestarter;

        splashdamage = firestarter.SplashDamageProjectile;
        splashrange = firestarter.SplashRangeProjectile;
        //otherEnemies = GameObject.FindGameObjectsWithTag( "Enemy" );
    }
    private void OnTriggerEnter( Collider other )
    {
        if ( other.gameObject.CompareTag( "ProjectileColl" ) )
        {
            //isProjectileConnected = true;
            Player.TakeDamage( firestarter.SplashDamageProjectile, false );

            firestarter.ExplodeVFX( transform );
            firestarter.attackSound.start();

            gameObject.SetActive( false );
            //firestarter.GetComponent<Firestarter>().DoSplashDamage( splashdamage, splashrange, transform );
        }
        else if ( other.gameObject.CompareTag( "Wall" ) )
            gameObject.SetActive( false );
    }
}
