using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public Transform player;
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

    RaycastHit hit;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerController").transform;
        currentHealth = baseHealth;
        currentSpeed = baseSpeed;
        viewAngle = spotlight.spotAngle;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        FindWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0f)
        {
            Dead();
        }

        if (CanSeePlayer())
        {
            StopAllCoroutines();
            Sight();
        }
        if (CanSeePlayer() == false)
        {
            Debug.Log("WHERE");
            FindWaypoint();
        }
    }


    public IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
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

    public void FindWaypoint()
    {
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    public void Sight()
    {
        Quaternion rotTarget = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTarget, rotationSpeed * Time.deltaTime);

        Physics.Raycast(barrel.position, barrel.TransformDirection(Vector3.forward), out hit, Mathf.Infinity);
        Debug.DrawRay(barrel.position, barrel.TransformDirection(Vector3.forward) * hit.distance, Color.cyan);
        Debug.Log(hit.collider.tag);
        if (hit.collider.tag == "Player")
        {
            Shoot();
        }


    }

    void Shoot()
    {
        Debug.Log("Shoot");
        audioManager.Play("EnemyShoot");
        //state.SwitchState(state.SightState);
        if (hit.collider.tag == "Player")
        {
            hit.collider.GetComponent<PlayerController>().TakeDamage(damage);
        }
       // else
       // {
       //     return;
       // }

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
