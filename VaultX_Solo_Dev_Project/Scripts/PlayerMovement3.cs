using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement3 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    //public Transform orientation;
    public float groundDrag = 1f;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool readyToJump = true;
    public KeyCode jumpKey = KeyCode.Space;

    public bool canMove;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public LayerMask ground;
    bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, ground);
        MyInput();
        SpeedControl();
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.Space) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput;

        if(isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 vel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (vel.magnitude > moveSpeed)
        {
            Vector3 limit = vel.normalized * moveSpeed;
            rb.velocity = new Vector3(limit.x, rb.velocity.y, limit.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
