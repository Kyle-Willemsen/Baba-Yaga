using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateManager : MonoBehaviour
{
    public BaseState currentState;
    public StaticState StaticState = new StaticState();
    public WanderState WanderState = new WanderState();
    public SightState SightState = new SightState();
    public ShootState ShootState = new ShootState();

    [HideInInspector] public Transform player;
    public Animator anim;
    public Transform viewTracker;
    public Transform barrel;
    [HideInInspector] public AudioManager audioManager;

    [Header("Enemy Stats")]
    public float baseHealth = 100;
    public float currentHealth;
    public float baseSpeed;
    public float currentSpeed;


    [Header("Paths")]
    public Transform pathHolder;
    public float waitTime;
    public float turnSpeed;
    public Light spotlight;
    public float viewDistance;
    float viewAngle;
    public LayerMask viewMask;
    public float rotationSpeed;


    public RaycastHit hit;
    float damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        //The first state the AI enters when created
        currentState = StaticState;

        //Reference to the AI's context
        currentState.EnterState(this);

        //anim = GetComponent<Animator>();
        player = GameObject.Find("PlayerController").transform;
        currentHealth = baseHealth;
        currentSpeed = baseSpeed;
        viewAngle = spotlight.spotAngle;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);

        if (currentHealth <= 0f)
        {
            Dead();
        }

        if (CanSeePlayer())
        {
            StopAllCoroutines();
            SwitchState(SightState);
        }

        Debug.Log(CanSeePlayer());
    }

    public void SwitchState(BaseState state)
    {
        currentState = state;

        state.EnterState(this);
    }

    public IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[0], currentSpeed * Time.deltaTime);

        int targetWaypointIndex = 0;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, currentSpeed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }



    public IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 directionToLookTargt = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(directionToLookTargt.z, directionToLookTargt.x) * Mathf.Rad2Deg;
        Debug.Log("TTF");
        while (Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) > 0.05f)
        {
            Debug.Log("TTF2");
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    public IEnumerator Shoot()
    {
        
        yield return new WaitForSeconds(1.5f);
        Debug.Log("ShootIIE");
        audioManager.Play("EnemyShoot");
        if (hit.collider.tag == "Player")
        {
            hit.collider.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public void Dead()
    {
        Destroy(gameObject);
        GameObject.Find("GameManager").GetComponent<TimeManager>().SlowMo();
    }


    public bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            float angleBetweenGuardAndGuard = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleBetweenGuardAndGuard < viewAngle / 2f)
            {
                if (!Physics.Linecast(transform.position, player.position, viewMask))
                {
                    return true;
                }
            }
        }
        return false;
    }


    public void SearchAnimComplete()
    {
        SwitchState(WanderState);
        anim.SetBool("isSearching", false);
    }
    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, 0.3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(viewTracker.position, transform.forward * viewDistance);
    }
}
