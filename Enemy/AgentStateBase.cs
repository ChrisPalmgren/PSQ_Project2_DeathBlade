using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.VFX;

public class AgentStateBase : StateMachineBehaviour
{

    ////////////////////////////////////////////////////////////
    // COMMON VARIABLES
    ////////////////////////////////////////////////////////////

    protected PlayerController Player = null;
    protected GameObject Enemy = null;
    protected EnemyBase EnemyBase = null;
    protected NavMeshAgent EnemyAgent = null;

    private bool isPlayerInSight = false;

    protected bool Activated = true;
    protected bool isSoundFinished = false;
    protected bool isAnimationFinished = false;
    protected bool isDead = false;
    protected bool hasShot = false;

    protected Vector3 newPosition = new Vector3();

    private GameObject tempProjectile = null;
    protected Transform gun = null;
    protected bool isCharging = false;
    protected VisualEffect ChargeUp = null;

    protected Ray myRay = new Ray();
    protected Ray EnemyRay = new Ray();

    protected int LastState;

    ////////////////////////////////////////////////////////////
    // INITIALIZE
    ////////////////////////////////////////////////////////////

    public void Awake()
    {
        Player = GameObject.Find( "Player" ).GetComponent<PlayerController>();
    }

    public override void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        Enemy = animator.gameObject;
        EnemyBase = Enemy.GetComponent<EnemyBase>();
        EnemyAgent = Enemy.GetComponent<NavMeshAgent>();

        if ( EnemyBase.isRanged == true )
        {
            if ( EnemyBase.finalform == true && EnemyBase.specialRange == false )
                gun = Enemy.transform.GetChild( 0 ).transform.GetChild( 2 ).transform.GetChild( 2 ).transform.GetChild( 0 ).transform.GetChild( 2 ).transform.GetChild( 0 ).transform.GetChild( 0 ).transform.GetChild( 0 ).transform.GetChild( 5 );
            else if ( EnemyBase.specialRange == true && EnemyBase.finalform == true )
            {
                gun = Enemy.transform.GetChild( 0 ).transform.GetChild( 1 ).transform.GetChild( 2 ).transform.GetChild( 0 ).transform.GetChild( 2 ).transform.GetChild( 0 ).transform.GetChild( 0 ).transform.GetChild( 0 ).transform.GetChild( 5 );
            }
            else
                gun = Enemy.transform.Find( "Gun" );

            if ( EnemyBase.finalform == true )
                ChargeUp = gun.transform.GetChild( 0 ).GetComponent<VisualEffect>();

            if ( gun == null )
            Debug.LogError( "gun object is null" );
        }
    }

    ////////////////////////////////////////////////////////////
    // ACTION FUNCTIONS
    ////////////////////////////////////////////////////////////

    public bool SeePlayer()
    {
        Vector3 enemyToPlayer = Player.transform.position - Enemy.transform.position;
        Ray ray = new Ray(Enemy.transform.position, enemyToPlayer);
        RaycastHit hit;
        Physics.Raycast( ray, out hit );
        if ( hit.transform.gameObject.CompareTag("Wall") == false && hit.transform.gameObject.CompareTag( "ExtraTagForEnemies" ) == false )
        {
                isPlayerInSight = true;
        }
        else
            isPlayerInSight = false;

        return isPlayerInSight;
    }

    public void ShootPlayer()
    {
        EnemyBase.attackSound.start();

        tempProjectile = EnemyBase.enemyBullets.GetEffect().gameObject;
        tempProjectile.SetActive( true );
        tempProjectile.GetComponent<EnemyProjectile>().Initialize( EnemyBase );
        tempProjectile.transform.position = gun.position;
        tempProjectile.transform.rotation = Quaternion.LookRotation( Player.transform.position - gun.position );
        Rigidbody tempRigidBody = tempProjectile.GetComponent<Rigidbody>();
        tempRigidBody.AddForce( gun.forward * EnemyBase.AttackSpeed );
    }

    IEnumerator FirestarterProjectile()
    {
        for ( int i = 0; i < 3; ++i )
        {
            tempProjectile = Instantiate( EnemyBase.projectile, gun.transform.position, gun.transform.rotation );
            tempProjectile.GetComponent<EnemyProjectile>().Initialize( EnemyBase );
            Rigidbody tempRigidBody = tempProjectile.GetComponent<Rigidbody>();
            tempRigidBody.AddForce( gun.transform.forward * EnemyBase.AttackSpeed );
            Destroy( tempProjectile, 10.0f );
            yield return new WaitForSeconds( 0.3f );
        }
    }

    public void KillEnemy( Animator animator )
    {
        Player.EnemyKilled( EnemyBase.EnergyRegen, EnemyBase.HealthRegen );

        animator.SetBool( "isDead", true );
        animator.enabled = false;
    }

    public bool KillPlayer()
    {
        if ( Player.health <= 0.0f )
        {
            return Activated = false;
        }
        else
            return Activated = true;
    }
}
