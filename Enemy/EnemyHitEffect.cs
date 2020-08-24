using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyHitEffect : MonoBehaviour
{
    ////////////////////////////////////////////////////////////
    // VARIABLES
    ////////////////////////////////////////////////////////////

    VisualEffect effect = null;
    Transform cameraTrans = null;
    Transform enemyParent = null;
    Vector3 effectPos = Vector3.zero;

    bool active = false;
    
    ////////////////////////////////////////////////////////////

    void Start()
    {
        cameraTrans = Camera.main.transform;
        effect = GetComponent<VisualEffect>();
    }
   
    ////////////////////////////////////////////////////////////

    void Update()
    {
        if( active == true )
        {
            if( enemyParent != null )
            {
                effectPos = enemyParent.position - cameraTrans.position;
                transform.position = enemyParent.position - effectPos.normalized + new Vector3( 0.0f, 0.97f, 0.0f );
            }
            else // enemy has been destroyed
            {
                transform.position = effectPos - effectPos.normalized;
            }
        }
    }

    ////////////////////////////////////////////////////////////
    
    public void Play( Transform enemy )
    {
        enemyParent = enemy;
        if ( effect != null )
            effect.Play();
        else
            // There is not a VFX component on this object
            Debug.LogError( "Error: Enemy hit effect is null! GO: " + gameObject.name );
        StartCoroutine( ChangeState() );
    }

    IEnumerator ChangeState()
    {
        active = true;
        yield return new WaitForSeconds( 2.0f );
        active = false;
    }
}
