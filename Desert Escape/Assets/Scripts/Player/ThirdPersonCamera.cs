using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform playerTransform; // Reference to the player's transform
    public float rotationSpeed = 2f; // Speed of camera rotation
    public float distanceFromPlayer = 2f; // Distance from the player
    public float heightOffset = 1f; // Height offset from the player

    private float mouseX, mouseY;

    private void LateUpdate()
    {
        // Get mouse input
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35f, 60f); // Clamp vertical rotation

        // Rotate camera around player
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 offset = new Vector3(0, heightOffset, -distanceFromPlayer);
        Vector3 targetPosition = playerTransform.position + rotation * offset;

        if (playerTransform.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            playerTransform.rotation = Quaternion.Euler(0, mouseX, 0);
        }

        transform.position = targetPosition;
        transform.LookAt(playerTransform.position + Vector3.up * heightOffset);
    }
}
