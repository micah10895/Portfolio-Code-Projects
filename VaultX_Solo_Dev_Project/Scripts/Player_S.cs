using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player_S : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float gravityScale = 1.0f;
    [SerializeField] private float maxDistanceRay = 5.0f;
    [SerializeField] private float rayLength = 1.0f;
    [SerializeField] private LayerMask layersToHit;
    private float jumpForce = 1.0f;
    Ray ray;
    Vector3 currVelocity;


    [SerializeField] private float rangeTest = 5f;

    Rigidbody rb;

    private void Jump()
    {
        jumpForce = Mathf.Sqrt(jumpHeight * -2f * (Physics.gravity.y));
        rb.AddForce (Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(0, 90, 0);

    }




    // Update is called once per frame
    private void Update() 
    {

        ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down), Color.magenta);

        if (Input.GetKey(KeyCode.Space))
        {


            if (IsGrounded())
            {
                Jump();
            }
            else if (!(Input.GetKey(KeyCode.A)) || !(Input.GetKey(KeyCode.D)) && !IsGrounded())
            {

            }


        }
        Movement();





    }

    private void FixedUpdate()
    {
        //changes the speed that the character falls
        //rb.AddForce(Physics.gravity * (gravityScale - 1.0f) * rb.mass);

    }

    private bool IsGrounded()
    {
        if(Physics.Raycast(ray, out RaycastHit hit, maxDistanceRay, layersToHit))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private void Movement()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x += 1;

        }


        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x -= 1;
        }

        inputVector = inputVector.normalized;

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;



        if (inputVector.y == 0f && inputVector.x == 0f)
        {

        }
        else
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
        }
    }
}
