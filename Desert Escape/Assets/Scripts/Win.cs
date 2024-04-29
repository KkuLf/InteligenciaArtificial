using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Win" tag
        if (other.CompareTag("Win"))
        {
            // Load the "Win" scene
            SceneManager.LoadScene("Win");
        }
    }
}
