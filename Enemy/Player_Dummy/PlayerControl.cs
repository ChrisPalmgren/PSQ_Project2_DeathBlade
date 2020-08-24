using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    GameObject Melee;
    GameObject Range;
    public bool isTrigger = false;

    public float Velocity = 100.0f;

    public float health;

    // Start is called before the first frame update
    void Start()
    {
        Melee = GameObject.Find( "Melee_Dummy" );
        Range = GameObject.Find( "Range_Dummy" );

        health = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if ( health <= 0.0f )
            gameObject.SetActive( false );

        if ( Input.GetKey( KeyCode.UpArrow ) )
            transform.position += transform.forward * Time.deltaTime * Velocity;
        if ( Input.GetKey( KeyCode.LeftArrow ) )
            transform.position -= transform.right * Time.deltaTime * Velocity;
        if ( Input.GetKey( KeyCode.DownArrow ) )
            transform.position -= transform.forward * Time.deltaTime * Velocity;
        if ( Input.GetKey( KeyCode.RightArrow ) )
            transform.position += transform.right * Time.deltaTime * Velocity;
    }

    //    if ( Input.GetKeyDown( KeyCode.K ) )
    //    {
    //        if (Melee != null)
    //            Melee.transform.GetChild( 0 ).gameObject.transform.GetChild( 0 ).gameObject.GetComponent<Slider>().value -= 10.0f;
    //    }

    //    if (Input.GetKeyDown(KeyCode.L))
    //    {
    //        if ( Range != null )
    //            Range.transform.GetChild( 0 ).gameObject.transform.GetChild( 0 ).gameObject.GetComponent<Slider>().value -= 10.0f;
    //    }

    //    if (Input.GetKeyDown( KeyCode.Space ))
    //    {
    //        if ( gameObject != null )
    //        {
    //            transform.GetChild( 0 ).gameObject.transform.GetChild( 0 ).gameObject.GetComponent<Slider>().value -= 10.0f;
    //            if ( transform.GetChild( 0 ).gameObject.transform.GetChild( 0 ).gameObject.GetComponent<Slider>().value == 0.0f )
    //                gameObject.SetActive(false);
    //        }
    //    }
    //}

    //private void OnTriggerEnter( Collider other )
    //{
    //    if ( other = GameObject.Find( "Dummy_Level" ).transform.GetChild( 1 ).gameObject.transform.GetChild( 0 ).GetComponent<Collider>() )
    //        isTrigger = true;
    //}
}
