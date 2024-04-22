using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float runSpeedMultiplier = 2f; // Speed multiplier when running
    public float crouchHeight = 0.5f; // Height when crouching

    public Animator animator; // Reference to the Animator component

    private CharacterController characterController;
    private Transform cameraTransform;
    private bool isCrouching = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform; // Assuming the main camera is the player's camera
    }

    private void Update()
    {
        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // Running
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed *= runSpeedMultiplier;
            animator.SetBool("IsRunning", true); // Set running animation
        }
        else
        {
            animator.SetBool("IsRunning", false); // Set not running animation
        }

        // Crouching
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;
            characterController.height = isCrouching ? crouchHeight : 2f; // Adjust character controller height
            animator.SetBool("IsCrouching", isCrouching); // Set crouching animation
        }

        // Camera Control
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX); // Rotate player horizontally
        cameraTransform.Rotate(Vector3.left * mouseY); // Rotate camera vertically

        // Apply movement
        characterController.Move(transform.TransformDirection(moveDirection) * currentMoveSpeed * Time.deltaTime);

        // Set walking animation
        animator.SetFloat("Speed", moveDirection.magnitude);
    }
}
