using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlchemistUI : MonoBehaviour
{
    [HideInInspector]
    public float visibility = 0.0f;
    [HideInInspector]
    public float cooldown = 1.0f;
    [SerializeField]
    Image image;

    bool onCooldown = false;

    public void ChangeVisibility( float amount )
    {
        if ( visibility >= 0.0f || amount <= 0.0f && onCooldown == false )
        {
            visibility -= amount;
            image.color = new Color( image.color.r, image.color.g, image.color.b, visibility );
            if ( visibility >= 0.8f && amount > 0.0f && onCooldown == false )
                StartCoroutine( CoolDown() );
        }
    }

    IEnumerator CoolDown()
    {
        onCooldown = true;
        yield return new WaitForSeconds( cooldown );
        onCooldown = false;
    }
}
