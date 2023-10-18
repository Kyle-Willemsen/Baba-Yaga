using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    PlayerController pController;
    AudioManager audioManager;
    public GameObject weapon;
    public Transform barrelPos;
    private bool canShoot;
    public LayerMask enemies;
    public float damage;
    RaycastHit hit;
    public float pentratingDistance;



    // Start is called before the first frame update
    void Start()
    {
        pController = GetComponent<PlayerController>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Aim();
        }
        else
        {
            weapon.SetActive(false);
            pController.currentSpeed = pController.baseSpeed;
        }


    }

    void Aim()
    {
        pController.currentSpeed = pController.baseSpeed / 2;
        weapon.transform.LookAt(pController.facingDir);
        weapon.SetActive(true);

        Physics.Raycast(barrelPos.transform.position, barrelPos.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity); 
        Debug.DrawRay(barrelPos.transform.position, barrelPos.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
        //Debug.Log("Hit " + hit.collider.name);

        if (hit.collider.tag == "Wall")
        {
            Debug.Log("Wall");
            float distanceToWall = Vector3.Distance(barrelPos.position, hit.collider.gameObject.transform.position);
            if (distanceToWall < pentratingDistance)
            {
                Debug.Log("Pen");
                Physics.Raycast(barrelPos.transform.position, barrelPos.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, LayerMask.GetMask("Enemy"));
                Debug.DrawRay(barrelPos.transform.position, barrelPos.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        audioManager.Play("PlayerShoot");
        CameraShake.Instance.ShakeCamera(1.5f, 0.075f);

        if (hit.collider.tag == "Enemy")
        {
            hit.collider.GetComponent<StateManager>().TakeDamage(damage);
        }
        else
        {
            return;
        }
    }
}
