using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 8.0f;
    public float gravity = 20.0f;
    public float rotationSpeed = 100.0f;

    public bool isGrounded = false;
    public bool isDef = false;
    public bool isDancing = false;
    public bool isWalking = false;
    public bool isJumping = false; // New flag for jumping

    private Animator animator;
    private CharacterController characterController;
    private Vector3 inputVector = Vector3.zero;
    private Vector3 targetDirection = Vector3.zero;
    private Vector3 moveDirection = Vector3.zero;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        Time.timeScale = 1;
    }

    void Update()
    {
        float z = Input.GetAxis("Horizontal");
        float x = Input.GetAxis("Vertical");

        animator.SetFloat("inputX", x);
        animator.SetFloat("inputZ", z);

        if (x != 0 || z != 0)
        {
            isWalking = true;
            animator.SetBool("isWalking", isWalking);
        }
        else
        {
            isWalking = false;
            animator.SetBool("isWalking", isWalking);
        }

        // Ground check
        isGrounded = characterController.isGrounded;

        if (isGrounded)
        {
            moveDirection = new Vector3(x, 0.0f, z);
            moveDirection *= speed;

            // Jump when the spacebar is pressed
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveDirection.y = jumpForce; // Apply jump force
                isJumping = true; // Set jumping flag
                animator.SetBool("isJumping", isJumping);
            }
        }
        else
        {
            // Apply gravity when the character is not grounded
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);

        // Reset jumping flag when grounded
        if (isGrounded && isJumping)
        {
            isJumping = false;
            animator.SetBool("isJumping", isJumping);
        }

        // Update movement based on input
        inputVector = new Vector3(x, 0, z);
        updateMovement();
    }

    void updateMovement()
    {
        Vector3 motion = inputVector;

        motion = ((Mathf.Abs(motion.x) > 1) || (Mathf.Abs(motion.z) > 1)) ? motion.normalized : motion;

        rotatTowardMovement();
        viewRelativeMovement();
    }

    void rotatTowardMovement()
    {
        if (inputVector != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void viewRelativeMovement()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        targetDirection = (Input.GetAxis("Horizontal") * right) + (Input.GetAxis("Vertical") * forward);
    }
}
