using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    protected PlayerController Player;
    protected EnemyBase Enemy;
    protected Rigidbody rb;

    protected virtual void Start()
    {
        Player = GameObject.Find( "Player" ).GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize( EnemyBase enemy )
    {
        Enemy = enemy;
        if ( rb == null )
            rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = false;
        StartCoroutine( DeactivateCollider() );
        StartCoroutine( DeactiveAfterTime() );
    }

    public virtual void DeInitialize()
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        gameObject.SetActive( false );
    }

    private void OnTriggerEnter( Collider other )
    {
        if ( other.gameObject.CompareTag( "ProjectileColl" ) )
        {
            Player.TakeDamage( Enemy.damage, false );
            DeInitialize();
        }
        else if ( other.gameObject.CompareTag( "Wall" ) )
            DeInitialize();
    }

    IEnumerator DeactivateCollider( )
    {
        gameObject.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds( 0.05f );
        gameObject.GetComponent<Collider>().enabled = true;
    }

    IEnumerator DeactiveAfterTime()
    {
        yield return new WaitForSeconds( 10.0f );
        DeInitialize();
    }
}
