using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation_S : MonoBehaviour
{

    Animator animator;
    int isJoggingHash;
    int isRunningHash;
    int velocityHash;
    float velocity = 0.0f;
    public float accelaration = 0.1f;
    public float decelaration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isJoggingHash = Animator.StringToHash("IsJogging");
        isRunningHash = Animator.StringToHash("IsRunning");
        velocityHash = Animator.StringToHash("Velocity");
    }

    // Update is called once per frame
    void Update()
    {
        bool isJogging = animator.GetBool(isJoggingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool forwardPressed = Input.GetKey(KeyCode.D);
        bool backwardPressed = Input.GetKey(KeyCode.A);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);


        if (forwardPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * accelaration;
        }
        else if (backwardPressed && velocity < 1.0f)
        {
            velocity += Time.deltaTime * accelaration;

        }
        else 
        {
            velocity = 0.0f;
        }
        //Debug.Log(velocity);
        //if (!forwardPressed && velocity > 0.0f)
        //{
        //    velocity -= Time.deltaTime * decelaration;
        //}
        if (!forwardPressed && velocity < 0.0f)
        {
            velocity = 0.0f;
        }

        animator.SetFloat(velocityHash, velocity);



        //if (!isJogging && forwardPressed)
        //{
        //    animator.SetBool(isJoggingHash, true);
        //}
        //if(isJogging && !forwardPressed)
        //{
        //    animator.SetBool(isJoggingHash, false);
        //}

        //if(!isRunning && (forwardPressed && runPressed))
        //{
        //    animator.SetBool(isRunningHash, true);
        //}
        //if(isRunning && (!forwardPressed || !runPressed))
        //{
        //    animator.SetBool(isRunningHash, false);
        //}
    }
}
