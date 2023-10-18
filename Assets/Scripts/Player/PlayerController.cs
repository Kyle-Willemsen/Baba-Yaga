using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    CharacterController controller;
    public float baseSpeed;
    public float currentSpeed;
    public bool canMove;
    public float maxHealth;
    public float currentHealth;
    Vector3 move;

    [Header("Gravity")]
    public float gravity;
    public LayerMask ground;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    [HideInInspector] public bool isGrounded;
    private Vector3 velocity;

    //Camera
    private Camera cam;
    [HideInInspector] public Vector3 facingDir;
    private Vector3 pointToLook;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        currentSpeed = baseSpeed;
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Mouse();
        Gravity();
    }

    void Move()
    {
        facingDir = new Vector3(pointToLook.x, transform.position.y, pointToLook.z);

        move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        controller.Move(move * Time.deltaTime * currentSpeed);

        if (move != Vector3.zero)
        {
            transform.LookAt(facingDir);
        }
    }

    private void Mouse()
    {
        Ray mousePos = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(mousePos, out rayLength))
        {
            pointToLook = mousePos.GetPoint(rayLength);

            transform.LookAt(facingDir);
            //gunSystem.shootPos.transform.LookAt(facingDir);
        }
    }

    private void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, ground);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}
