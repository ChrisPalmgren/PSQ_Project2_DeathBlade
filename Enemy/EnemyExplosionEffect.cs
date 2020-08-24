using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyExplosionEffect : MonoBehaviour
{
    ////////////////////////////////////////////////////////////
    // VARIABLES
    ////////////////////////////////////////////////////////////

    VisualEffect effect = null;
    Transform detonationSpot = null;
    Vector3 effectPos = Vector3.zero;

    bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        effect = GetComponent<VisualEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( active == true )
        {
            if ( detonationSpot != null )
            {
                transform.position = new Vector3(detonationSpot.position.x, 0.0f, detonationSpot.position.z);
                active = false;
            }
        }
    }

    public void Play( Transform detonation )
    {
        detonationSpot = detonation;
        effect.Play();
        StartCoroutine( ChangeState() );
    }

    IEnumerator ChangeState()
    {
        active = true;
        yield return new WaitForSeconds( 2.0f );
        active = false;
    }
}
