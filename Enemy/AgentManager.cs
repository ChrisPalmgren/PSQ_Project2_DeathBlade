using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
    private Animator anim;

    private GameObject player;
    private GameObject healthbar;
    private GameObject player_healthbar;

    private NavMeshAgent Agent;
    public Agent_State currentState;

    public float AttackSpeed = 1.0f;

    public float inRoomTime = 5.0f;
    public float AlertTime = 1.0f;
    private float dt_temp;

    public float detectXpos = 30.0f;
    public float detectXneg = -30.0f;
    public float detectYpos = 30.0f;
    public float detectYneg = -30.0f;

    private float distance;
    private Vector3 huntTarget;

    Vector3 vDestination;

    ////////////////////////////////////////////////////////////
    // START
    ////////////////////////////////////////////////////////////
    void Start()
    {
        anim = GetComponent<Animator>();

        healthbar = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        healthbar.GetComponent<Slider>().maxValue = 75.0f + 25.0f/*   * floor/level   */;
        healthbar.GetComponent<Slider>().value = healthbar.GetComponent<Slider>().maxValue;

        player = GameObject.Find("Player_Dummy");
        player_healthbar = player.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        player_healthbar.GetComponent<Slider>().maxValue = 100.0f;
        player_healthbar.GetComponent<Slider>().value = healthbar.GetComponent<Slider>().maxValue;

        Agent = GetComponent<NavMeshAgent>();
        Agent.speed = 5.0f;
        Agent.angularSpeed = 200.0f;
        Agent.acceleration = 50.0f;
        currentState = Agent_State.idle;
        vDestination = Vector3.zero;
    }
    ////////////////////////////////////////////////////////////
    // UPDATE
    ////////////////////////////////////////////////////////////
    void Update()
    {
        if (TakeDamage() <= 0.0f)
        currentState = Agent_State.death;

        if (player != null)
        {
            distance = Vector3.Distance(transform.position, player.transform.position);
            huntTarget = player.transform.position - transform.position;
            Debug.Log(distance.ToString());
        }
        ////////////////////////////////////////////////////////////
        // STATE EVENTS
        ////////////////////////////////////////////////////////////
        switch ( currentState)
        {
            ////////////////////////////////////////////////////////////
            // IDLE
            ////////////////////////////////////////////////////////////
            case Agent_State.idle:
                Debug.Log("Idle");
                if (player != null)
                {
                    Debug.DrawRay(transform.position, huntTarget, Color.green);
                    Debug.DrawRay(transform.position, transform.forward * 10.0f);

                    Agent.isStopped = true;
                    dt_temp += Time.deltaTime;
                    if (Alert() == true || dt_temp >= inRoomTime)
                    {
                        dt_temp = 0.0f;
                        currentState = Agent_State.alert;
                    }
                }
                break;
            ////////////////////////////////////////////////////////////
            // ALERT
            ////////////////////////////////////////////////////////////
            case Agent_State.alert:
                Debug.Log("Alert");
                Debug.DrawRay(transform.position, huntTarget, Color.yellow);

                anim.SetBool("isAlerting", true);

                transform.forward = player.transform.position - transform.position;
                Debug.Log( transform.forward.ToString() );
                dt_temp += Time.deltaTime;
                if (dt_temp >= AlertTime)
                {
                    dt_temp = 0.0f;
                    //currentState = Agent_State.chase;
                }

                break;
            ////////////////////////////////////////////////////////////
            // CHASE
            ////////////////////////////////////////////////////////////
            case Agent_State.chase:
                Debug.Log("Chase");
                Debug.DrawRay(transform.position, huntTarget, Color.yellow);
                Debug.DrawRay(transform.position, transform.forward * 10.0f);

                anim.SetBool("isChasing", true);

                Agent.isStopped = false;
                Agent.destination = player.transform.position;
                Agent.speed = 15.0f;
                if (Vector3.Distance(transform.position, player.transform.position) < 10.0f)
                {
                    Agent.isStopped = true;
                    currentState = Agent_State.attack;
                }
                break;
            ////////////////////////////////////////////////////////////
            // ATTACK
            ////////////////////////////////////////////////////////////
            case Agent_State.attack:
                Debug.Log("Attack");
                Debug.DrawRay(transform.position, huntTarget, Color.red);
                transform.forward = player.transform.position - transform.position;
                if (Vector3.Distance(transform.position, player.transform.position) > 10.0f)
                {
                    currentState = Agent_State.chase;
                }
                if (DoDamage() <= 0.0f)
                {
                    Object.Destroy(player);
                    distance = 0.0f;
                    huntTarget = Vector3.zero;
                    currentState = Agent_State.idle;
                }
                break;
            ////////////////////////////////////////////////////////////
            // DEATH
            ////////////////////////////////////////////////////////////
            case Agent_State.death:
                Debug.Log("Death");
                Agent.isStopped = true;
                Object.Destroy(gameObject);
                break;
        }
    }

    ////////////////////////////////////////////////////////////
    // DO DAMAGE
    ////////////////////////////////////////////////////////////
    public float DoDamage()
    {
        dt_temp += Time.deltaTime;
        if (dt_temp >= AttackSpeed)
        {
            dt_temp = 0.0f;
            player_healthbar.GetComponent<Slider>().value -= 10.0f;
        }

        return player_healthbar.GetComponent<Slider>().value;
    }

    ////////////////////////////////////////////////////////////
    // TAKE DAMAGE
    ////////////////////////////////////////////////////////////
    public float TakeDamage()
    {
        if (Input.GetKeyDown(KeyCode.K))
            healthbar.GetComponent<Slider>().value -= 10.0f;

        return healthbar.GetComponent<Slider>().value;
    }

    ////////////////////////////////////////////////////////////
    // ALERT FUNCTION
    ////////////////////////////////////////////////////////////
    public bool Alert()
    {
        Vector3 playerPos = player.transform.position;

        if (playerPos.x <= detectXpos && playerPos.x >= detectXneg && playerPos.z <= detectYpos && playerPos.z >= detectYneg)
            return true;

        return false;
    }

    public enum Agent_State
    {
        idle,
        alert,
        chase,
        attack,
        death
    }
}


////////////////////////////////////////////////////////////
// GARBAGE
////////////////////////////////////////////////////////////
//
//case Agent_State.idle:
//                Debug.Log("Idle");
//                Debug.DrawRay(transform.position, huntTarget, Color.green);
//                Debug.DrawRay(transform.position, transform.forward* 10.0f);

//                SpotPlayer();
//fTime += Time.deltaTime;
//                Debug.Log(fTime.ToString());
//                if (fTime > 3.0f)
//                {
//                    fTime = 0.0f;
//                    currentState = Agent_State.wander;
//                }
//                break;
//            case Agent_State.wander:
//                Debug.Log("Wander");
//                Debug.DrawRay(transform.position, huntTarget, Color.green);
//                Debug.DrawRay(transform.position, transform.forward* 10.0f);

//                SpotPlayer();
//                if (vDestination == Vector3.zero)
//                {
//                    vDestination = new Vector3(Random.Range(-32, 32), 0.0f, Random.Range(-32, 32));
//                    Agent.destination = vDestination;
//                }
//                if (Vector3.Distance(transform.position, Agent.destination) < 5.0f)
//                {
//                    vDestination = Vector3.zero;
//                    currentState = Agent_State.idle;
//                }
//                break;

//private void SpotPlayer()
//{
//    Vector3 enemyToPlayer = player.transform.position - transform.position;

//    Ray ray = new Ray(transform.position, enemyToPlayer);
//    RaycastHit hit;

//    Physics.Raycast( ray, out hit );
//    if ( hit.transform.position == player.transform.position )
//    {
//        enemyToPlayer.Normalize();
//        Vector3 EnemyForwardNormalized = transform.forward.normalized;

//        float dotProd = Vector3.Dot(EnemyForwardNormalized, enemyToPlayer);

//        if ( dotProd > 0.75f )
//        {
//            currentState = Agent_State.chase;
//        }
//    }
//}