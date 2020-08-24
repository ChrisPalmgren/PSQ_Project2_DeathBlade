using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class EnemyBase : MonoBehaviour
{
    ////////////////////////////////////////////////////////////
    // VARIABLES
    ////////////////////////////////////////////////////////////
    public bool finalform;
    public bool specialRange;
    [Header("Variables"), Tooltip(" Enemy Health ")]
    public float health;
    [Tooltip(" Enemy Damage ")]
    public float damage;
    [Tooltip(" Run Speed ")]
    public float runspeed;
        
    ////////////////////////////////////////////////////////////

    [Tooltip(" Alert Duration ")]
    public float AlertTime = 3.0f;
    [Tooltip(" Time Undetected ")]
    public float InRoomTime = 5.0f;
    [Tooltip(" Enemy Distance ")]
    public float EnemyDistance;
    
    ////////////////////////////////////////////////////////////

    [Tooltip(" Ranged Enemy ")]
    public bool isRanged;
    [Tooltip(" Enemy Projectile Prefab ")]
    public GameObject projectile;

    [Tooltip(" Attack Speed ")]
    public float AttackSpeed = 1500.0f;
    [Tooltip(" Attack Rate ")]
    public float AttackRate = 1.0f;

    [SerializeField, Range(0, 100), Tooltip("Player will heal this % of damage done to enemy")]
    protected float lifestealPercentage = 0.0f;

    [Tooltip(" Health Regenerator ")]
    public float HealthRegen;
    [Tooltip(" Energy Regenerator ")]
    public float EnergyRegen;

    [SerializeField]
    protected float dissolveSpeed = 1.0f;

    ////////////////////////////////////////////////////////////

    [Header("Components & Objects"), Tooltip(" SFX: Death ")]
    public AudioClip SFX_Death = null;

    [SerializeField]
    protected GameObject Enemy = null; // model of the enemy
    [SerializeField]
    protected List<GameObject> materialParents = new List<GameObject>();

    [SerializeField, Tooltip("Dissolve materials for death. MUST MATCH MATERIALS ON ENEMY OBJECT")]
    protected List<Material> deathMaterials = new List<Material>();

    ////////////////////////////////////////////////////////////

    [Header("Knockback"), SerializeField, Tooltip("How big is the knockback force?")]
    protected float knockbackSpeed = 1.0f;
    [SerializeField, Tooltip("How long will the knockback effect last for?")]
    protected float knockbackLength = 1.0f;

    ////////////////////////////////////////////////////////////

    [Header("Scaling Variables"), SerializeField]
    protected float healthScaling = 0;
    [SerializeField]
    protected float attackScaling = 0;
    [SerializeField]
    protected float speedScaling = 0;

    ////////////////////////////////////////////////////////////
    // HIDDEN VARIABLES
    ////////////////////////////////////////////////////////////

    [HideInInspector]
    // Player detection bool
    public bool isPlayerDetected = false;

    // Enemy knockback bool
    [HideInInspector]
    public bool isHit = false;

    // var for ShootEvent
    [HideInInspector]
    public bool shoot = false;

    protected Animator animator = null;
    protected PlayerController player = null;
    protected NavMeshAgent navAgent = null;
    protected GameManager gameManager = null;
    protected LevelManager levelManager = null;
    protected Transform cameraTrans = null;

    protected Collider[] colliders = null;
    protected List<Material[]> originalMaterials = new List<Material[]>();

    protected VFXList enemyHitEffects = null;
    [HideInInspector]
    public VFXList enemyBullets = null;

    protected bool isDissolving = false;
    [HideInInspector]
    public bool active = false;
    [HideInInspector]
    public bool hasAlerted = false;

    protected float dissolveAmount = -1;

    protected int floor = 0;
    [HideInInspector]
    public int roomIndex = -1;

    ////////////////////////////////////////////////////////////
    // SOUND VARIABLES
    ////////////////////////////////////////////////////////////

    protected FMOD.Studio.EventInstance hitSound;
    protected FMOD.Studio.EventInstance disappearsSound;
    protected FMOD.Studio.EventInstance deadSound;
    [HideInInspector] public FMOD.Studio.EventInstance anticipationSound;
    [HideInInspector] public FMOD.Studio.EventInstance attackSound;

    ////////////////////////////////////////////////////////////

    protected virtual void Start()
    {
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        levelManager = GameObject.Find( "LevelManager" ).GetComponent<LevelManager>();
        if( transform.position.y > 3.0f )
        {
            Debug.LogError( "Error: " + gameObject.name + " spawned above floor position! Room Index: " + roomIndex + " Make spawners smaller and closer to the floor." );
            //gameManager.enemiesKilledNeededToGetThreeUpgrades--;
            //if ( levelManager.spawnedEnemies.Contains( gameObject ) == true )
            //    levelManager.spawnedEnemies.Remove( gameObject );
            //gameManager.SetKillText();
            //Destroy( gameObject );
            return;
        }

        ////////////////////////////////////////////////////////////
        // GET COMPONENTS
        ////////////////////////////////////////////////////////////

        if ( materialParents.Count != 0 )
        {
            foreach ( GameObject materialParent in materialParents )
            {
                //Material[] newMaterial = materialParent.GetComponent<Renderer>().materials;
                originalMaterials.Add( materialParent.GetComponent<Renderer>().materials );
            }
        }
        else
        {
            Debug.LogError( "Error: Materialparents not set up for " + gameObject.name );
        }

        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find( "Player" ).GetComponent<PlayerController>();
        enemyHitEffects = GameObject.Find( "EnemyHit VFXs" ).GetComponent<VFXList>();
        enemyBullets = GameObject.Find( "Enemy Bullets" ).GetComponent<VFXList>();
        cameraTrans = Camera.main.transform;
        colliders = GetComponents<Collider>();

        ////////////////////////////////////////////////////////////
        // APPLY FLOOR SCALING
        ////////////////////////////////////////////////////////////

        floor = GameObject.Find( "LevelManager" ).GetComponent<LevelManager>().floor;
        health += healthScaling * floor;
        damage += attackScaling * floor;
        navAgent.speed += speedScaling * floor;

        AttachSound();
        ChangeState( false );
    }

    protected virtual void AttachSound()
    {
        ////////////////////////////////////////////////////////////
        // AUDIO
        ////////////////////////////////////////////////////////////

        Rigidbody rb = GetComponent<Rigidbody>();

        hitSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_hit" );
        RuntimeManager.AttachInstanceToGameObject( hitSound, transform, rb );
        deadSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_dead" );
        RuntimeManager.AttachInstanceToGameObject( deadSound, transform, rb );
        disappearsSound = RuntimeManager.CreateInstance( "event:/Grunt/enemy_disappears" );
        RuntimeManager.AttachInstanceToGameObject( disappearsSound, transform, rb );

        if ( isRanged == true )
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Grunt/ranged_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Grunt/ranged_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }
        else
        {
            anticipationSound = RuntimeManager.CreateInstance( "event:/Grunt/melee_anticipation" );
            RuntimeManager.AttachInstanceToGameObject( anticipationSound, transform, rb );
            attackSound = RuntimeManager.CreateInstance( "event:/Grunt/melee_attack" );
            RuntimeManager.AttachInstanceToGameObject( attackSound, transform, rb );
        }

        ////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////

    protected virtual void Update()
    {
        if( active == false )
            return;

        if( isDissolving == true )
        {
            foreach ( GameObject materialParent in materialParents )
                foreach ( Material material in materialParent.GetComponent<Renderer>().materials )
                    material.SetFloat( "_Dissolve_amount", dissolveAmount );

            dissolveAmount += Time.deltaTime * dissolveSpeed;
            if ( dissolveAmount >= 1.0f )
                Destroy( gameObject );
        }
    }

    ////////////////////////////////////////////////////////////

    public void DealDamage()
    {
        if ( Vector3.Distance( Enemy.transform.position, player.transform.position) < EnemyDistance )
            player.TakeDamage( damage, true );
    }

    public void ShootEvent()
    {
        shoot = true;
    }

    public virtual void TakeDamage( float damage )
    {
        ////////////////////////////////////////////////////////////

        if ( player == null )
        {
            Debug.LogError( "Player is null " );
            return;
        }
        if ( enemyHitEffects == null )
        {
            Debug.LogError( "Hiteffects is null " );
            return;
        }
        if ( levelManager == null )
        {
            Debug.LogError( "levelManager is null " );
            return;
        }

        health -= damage;
        player.RefillHealth( lifestealPercentage * damage );
        AlertRoom();

        // "Spawn" hit effect
        EnemyHitEffect hitEffect = enemyHitEffects.GetEffect().GetComponent<EnemyHitEffect>();
        hitEffect.Play( transform );
        hitSound.start();

        if ( health <= 0.0f )
        {
            deadSound.start();

            ////////////////////////////////////////////////////////////
            // CHANGE MATERIALS TO DISSOLVE
            ////////////////////////////////////////////////////////////
            // Materials returned by renderer is a copy, not a reference

            if ( materialParents.Count != 0 )
            {
                int materialParentIndex = 0;
                int deathMaterialIndex = 0;
                try
                {
                    foreach ( GameObject materialParent in materialParents )
                    {
                        Material[] materials = new Material[ originalMaterials[ materialParentIndex ].Length ];
                        for ( int materialIndex = 0; materialIndex < originalMaterials[ materialParentIndex ].Length; materialIndex++ )
                        {
                            Texture originalTexture = originalMaterials[ materialParentIndex ][ materialIndex ].GetTexture( "_BaseColorMap" );
                            if ( originalTexture == null )
                            {
                                Debug.LogError( "Error: Enemy original texture is null. GO: " + gameObject.name );
                                navAgent.enabled = false;
                                gameManager.KillEnemy();
                                Destroy( gameObject );
                                break;
                            }

                            materials[ materialIndex ] = deathMaterials[ deathMaterialIndex ];
                            materials[ materialIndex ].SetTexture( "_DissolveAlbedo", originalTexture );
                            deathMaterialIndex++;
                        }

                        materialParents[ materialParentIndex ].GetComponent<Renderer>().materials = materials;
                        isDissolving = true;
                        disappearsSound.start();
                        materialParentIndex++;
                    }
                }
                catch
                {
                    Debug.LogError( "Error: Enemy dissolve fail. GO: " + gameObject.name );
                    navAgent.enabled = false;
                    gameManager.KillEnemy();
                    Destroy( gameObject );
                }
                
            }
            else
            {
                Debug.LogError( "Error: No material parents on enemy. GO: " + gameObject.name );
                navAgent.enabled = false;
                gameManager.KillEnemy();
                Destroy( gameObject );
            }

            ////////////////////////////////////////////////////////////
            // PROPERLY KILL ENEMY FROM GAME
            ////////////////////////////////////////////////////////////

            navAgent.enabled = false;
            foreach ( Collider collider in colliders )
                collider.enabled = false;
            gameManager.KillEnemy();

            ////////////////////////////////////////////////////////////
        }

        ////////////////////////////////////////////////////////////

        else
        {
            //StartCoroutine( FlashColor() );
        }

        ////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////

    protected virtual IEnumerator FlashColor()
    {
        ////////////////////////////////////////////////////////////
        
        for ( int i = 0; i < 4; ++i )
        {
            Enemy.GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds( 0.1f );
            Enemy.GetComponent<Renderer>().enabled = true;
            yield return new WaitForSeconds( 0.1f );
        }
        
        ////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////

    public void FirestarterShoot(IEnumerator shootshoot)
    {
        StartCoroutine( shootshoot );
    }

    ////////////////////////////////////////////////////////////

    public void AlertRoom()
    {
        if( hasAlerted == false )
        {
            hasAlerted = true;
            levelManager.ActivateEnemiesInRoom( roomIndex, true );
        }
    }

    public void ChangeState( bool state )
    {
        active = state;
        if( active == false )
        {
            animator.speed = 0.0f; // freeze animation
        }
        else
        {
            if( animator != null )
            animator.speed = 1.0f; // freeze animation
        }
    }

    ////////////////////////////////////////////////////////////

    public virtual IEnumerator Knockback()
    {
        if ( animator != null )
        {
            if ( animator != null )
                animator.Play( "KnockBack" );
            //    animator.speed = 0.0f; // freeze animation
            if ( navAgent != null )
                navAgent.velocity = player.transform.forward * knockbackSpeed; // set force in same direction of player forward
            yield return new WaitForSeconds( knockbackLength );
            //if ( animator != null )
            //    animator.speed = 1.0f; // unfreeze animation
        }
    }

    ////////////////////////////////////////////////////////////
}
