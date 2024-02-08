using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_s2 : MonoBehaviour
{
    float playerHeight = 2f;

    [Header("Movement")]
    public float groundMoveSpeed = 6f;
    public float airMoveSpeed = 3f;
    float moveSpeed;

    [Header("Jumping")]
    //float jumpForce = 5f;
    //public float jumpHeight = 5f;
    //public float jumpCoolDown = 0.5f;
    //public float airMultiplier = 10f;
    //bool readyToJump;
    //public KeyCode jumpKey = KeyCode.Space;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    bool isGrounded;

    Vector3 moveDirection;

    Rigidbody rb;
    Ray ray;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //FixAirSpeed();
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);


        MyInput();
        //ControlDrag();

        //if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        //{
        //    Debug.Log("Jump");
        //    Jump();

        //}
        MovePlayer();
        //SpeedControl();


    }

    private void MyInput()
    {
        //horizontalMovement = Input.GetAxisRaw("Vertical"); //lateral movement
        verticalMovement = Input.GetAxisRaw("Horizontal"); //forward movement
        //verticalMovement = Input.GetAxis("Horizontal");

         moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

        //if(Input.GetKey(jumpKey) && readyToJump && isGrounded)
        //{
        //    readyToJump = false;
        //    Jump();
        //    Debug.Log("jump");
        //    Invoke(nameof(ResetJump), jumpCoolDown);
        //}
    }

    //void Jump()
    //{
    //    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);


    //    jumpForce = Mathf.Sqrt(jumpHeight * -2f * (Physics.gravity.y));
    //    rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    //}

    //void ResetJump()
    //{
    //    readyToJump = true;
    //}

    void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;
            //moveSpeed = groundMoveSpeed;
        }
        else
        {
            rb.drag = airDrag;
            //moveSpeed = airMoveSpeed;
        }
    }

    //private void FixedUpdate()
    //{
    //    //MovePlayer();
    //}

    void MovePlayer()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            //rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        //transform.Translate(Vector3.forward * Time.deltaTime * verticalMovement);
    }

    //void FixAirSpeed()
    //{
    //    airMoveSpeed = (groundMoveSpeed * airDrag) / groundDrag;
    //}

    void SpeedControl()
    {
        Vector3 current = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (current.magnitude > groundMoveSpeed)
        {
            Vector3 limit = current.normalized * groundMoveSpeed;
            rb.velocity = new Vector3(limit.x, rb.velocity.y, limit.z);
        }
    }
}
