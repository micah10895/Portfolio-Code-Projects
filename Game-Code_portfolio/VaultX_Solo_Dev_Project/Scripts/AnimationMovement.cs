//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.UIElements;

//public class AnimationMovement : MonoBehaviour
//{

//    PlayerInput playerInput;
//    CharacterController characterController;
//    Animator animator;

//    int isMovingHash;
    
//    Vector2 currentMovementInput;
//    Vector3 currentMovement;
//    bool isMovementPressed;
//    float rotationFactorPerFrame = 15.0f;

//    float gravity = -9.81f;
//    float groundedGravity = -.05f;

//    bool isJumpPressed = false;
//    float initialJumpVelocity;
//    float maxJumpHeight = 4.0f;
//    float maxJumpTime = 1.0f;
//    bool isJumping = false;


//    private void Awake()
//    {
//        playerInput = new PlayerInput();
//        characterController = GetComponent<CharacterController>();
//        animator = GetComponent<Animator>();

//        isMovingHash = Animator.StringToHash("isMoving");

//        playerInput.CharacterControls.Move.started += onMOvementInput;
//        playerInput.CharacterControls.Move.canceled += onMOvementInput;
//        playerInput.CharacterControls.Move.performed += onMOvementInput;
//        playerInput.CharacterControls.Jump.started += onJump;
//        playerInput.CharacterControls.Jump.canceled += onJump;

//        SetupJumpVariables();
//    }

//    void SetupJumpVariables()
//    {
//        float timeToApex = maxJumpTime / 2;
//        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
//        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
//    }

//    void HandleJump()
//    {
//        if(!isJumping && characterController.isGrounded && isJumpPressed)
//        {
//            isJumping = true;
//            currentMovement.y = initialJumpVelocity * .5f;
//        }
//        else if (!isJumpPressed && isJumping && characterController.isGrounded)
//        {
//            isJumping = false;
//        }
//    }

//    void onJump(InputAction.CallbackContext context)
//    {
//        isJumpPressed = context.ReadValueAsButton();
//        Debug.Log(isJumpPressed);
//    }

//    void onMOvementInput(InputAction.CallbackContext context)
//    {
//        currentMovementInput = context.ReadValue<Vector2>();
//        currentMovement.x = currentMovementInput.x;
//        //currentMovement.z = currentMovementInput.y;
//        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
//    }

//    void HandleRotation()
//    {
//        Vector3 positionToLookAt;

//        positionToLookAt.x = currentMovement.x;
//        positionToLookAt.y = 0.0f;
//        positionToLookAt.z = currentMovement.z;

//        Quaternion currentRotation = transform.rotation;
//        if (isMovementPressed)
//        {
//            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
//            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
//        }
//    }

//    void HandleAnimation()
//    {
//        bool isMoving = animator.GetBool(isMovingHash);

//        if(isMovementPressed && !isMoving)
//        {
//            animator.SetBool(isMovingHash, true);
//        }
//        else if(!isMovementPressed && isMoving) 
//        {
//            animator.SetBool(isMovingHash, false);
//        }
//    }

//    void HandleGravity()
//    {
//        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
//        float fallMultiplier = 2.0f;

//        if (characterController.isGrounded)
//        {
//            currentMovement.y = groundedGravity;
//        }
//        else if (isFalling)
//        {
//            float previousYVelocity = currentMovement.y;
//            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
//            float nextYVelocity = (previousYVelocity + newYVelocity) * .5f;
//            currentMovement.y = nextYVelocity;
//        }
//        else
//        {
//            //framerate dependent gravity for jumps
//            float previousYVelocity = currentMovement.y;
//            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
//            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * .5f, -20.0f);
//            currentMovement.y = nextYVelocity;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        HandleRotation();
//        HandleAnimation();
//        characterController.Move(currentMovement * Time.deltaTime);
//        HandleGravity();
//        HandleJump();
//    }

//    private void OnEnable()
//    {
//        playerInput.CharacterControls.Enable();
//    }

//    private void OnDisable()
//    {
//        playerInput.CharacterControls.Disable();
//    }
//}
