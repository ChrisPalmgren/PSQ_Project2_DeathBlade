using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Firestarter : EnemyBase
{
    [Header("Firestarter Variables"), Tooltip(" Splash-Damage on Death ")]
    public float SplashDamageDeath;
    [Tooltip(" Splash-Range on Death ")]
    public float SplashRangeDeath;
    [Tooltip(" Splash-Damage on Projectile ")]
    public float SplashDamageProjectile;
    [Tooltip(" Splash-Range on Projectile ")]
    public float SplashRangeProjectile;

    [Header("Firestarter FX")]
    [SerializeField, Tooltip("Fire Materials")]
    List<Material> fireMaterials = new List<Material>();

    [HideInInspector]
    public bool isFirestarterAttacking = false;
    private LayerMask EnemyLayer;
    private Material[] originalFirestarterMaterials = null;
    private float explodeTime = 0.0f;
    private float fireAmount = -1.0f;
    private float fireSpeed = 1.0f;
    private VFXList enemyExplosionEffects = null;

    private bool isDead = false;
    private bool isMeleeConnected = false;
    private bool isAlreadyDetonated = false;

    [HideInInspector] public FMOD.Studio.EventInstance anticipationSound2;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        enemyBullets = GameObject.Find( "Firestarter Bullets" ).GetComponent<VFXList>();
        enemyExplosionEffects = GameObject.Find( "Explosion VFXs" ).GetComponent<VFXList>();

        EnemyLayer = ( 1 << LayerMask.NameToLayer( "Enemy" ) );

        if ( isRanged == true )
            gameObject.GetComponent<Collider>().isTrigger = false;
        else
        {
            EnemyLayer = ( 1 << LayerMask.NameToLayer( "Enemy" ) );
            gameObject.GetComponent<Collider>().isTrigger = true;
        }
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
        deadSound = RuntimeManager.CreateInstance( "event:/Specter/specter_death" );
        RuntimeManager.AttachInstanceToGameObject( deadSound, transform, rb );
        disappearsSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_disappears" );
        RuntimeManager.AttachInstanceToGameObject( disappearsSound, transform, rb );

        if ( isRanged == true )
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Firestarter/firestarter_ranged_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Firestarter/firestarter_ranged_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }
        else
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Firestarter/firestarter_melee_anticipation_1" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            anticipationSound2 = RuntimeManager.CreateInstance( "event:/Firestarter/firestarter_melee_anticipation_2" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound2, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Firestarter/firestarter_explosion_melee" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }

        ////////////////////////////////////////////////////////////
    }

    protected override void Update()
    {
        base.Update();

        if ( isDead == true )
            return;

        if ( health <= 0 )
        {
            ExplodeVFX( transform );
            DoSplashDamage( SplashDamageDeath, SplashRangeDeath, transform);
            gameManager.KillEnemy();
            navAgent.enabled = false;
            isDead = true;
        }

        //if ( isFirestarterAttacking == true )
        //{
        //    ////////////////////////////////////////////////////////////
        //    // CHANGE MATERIALS TO FIRE UP
        //    ////////////////////////////////////////////////////////////
        //    // Materials returned by renderer is a copy, not a reference

        //    try
        //    {
        //        Material[] materials = new Material[ fireMaterials.Count ];
        //        for (int materialIndex = 0; materialIndex < fireMaterials.Count; ++materialIndex )
        //        {
        //            Texture originalTexture = originalMaterials[ materialIndex ].GetTexture( "_BaseColorMap" );
        //            if( originalTexture == null )
        //            {
        //                Debug.LogWarning( "Enemy fire fucked up " + gameObject.name );
        //                break;
        //            }

        //            materials[ materialIndex ] = fireMaterials[ materialIndex ];
        //            materials[ materialIndex ].SetTexture( "_MainTexture", originalTexture );
        //        }
        //        Enemy.GetComponent<Renderer>().materials = materials;
        //    }
        //    catch
        //    {
        //        Debug.LogWarning( "Enemy fire fucked up " + gameObject.name );
        //    }

        //    if ( isFirestarterAttacking == true)
        //    {
        //        foreach ( Material material in Enemy.GetComponent<MeshRenderer>().materials )
        //            material.SetFloat( "_NormalStrength", fireAmount );

        //        fireAmount += Time.deltaTime * fireSpeed;
        //        if ( fireAmount >= 1.0f )
        //            isFirestarterAttacking = false;
        //    }
        //}
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( gameObject.GetComponent<Collider>().isTrigger == true )
        {
            if ( other.gameObject.CompareTag( "Player" ) )
            {
                if ( isAlreadyDetonated == true )
                    return;

                attackSound.start();
                ExplodeVFX( transform );
                player.TakeDamage( SplashDamageDeath, true );
                gameObject.SetActive( false );
                isAlreadyDetonated = true;
            }
        }
    }

    public void ExplodeVFX( Transform transform )
    {
        if ( enemyExplosionEffects == null )
        {
            Debug.LogError( "Explosion effect is null" );
            return;
        }
        EnemyExplosionEffect hitEffect = enemyExplosionEffects.GetEffect().GetComponent<EnemyExplosionEffect>();
        hitEffect.Play( transform );
    }

    public void DoSplashDamage( float splashdamage, float splashrange, Transform detonationPos )
    {
        if ( Vector3.Distance( player.transform.position, detonationPos.position ) < splashrange )
            player.TakeDamage(splashdamage, false);

        Collider[] EnemyArray = Physics.OverlapSphere(Enemy.transform.position, splashrange, EnemyLayer);
        for ( int i = 0; i < EnemyArray.Length; ++i )
        {
            float debugHealth = EnemyArray[ i ].GetComponent<EnemyBase>().health;
            EnemyArray[ i ].GetComponent<EnemyBase>().TakeDamage( splashdamage );
        }
    }
}
