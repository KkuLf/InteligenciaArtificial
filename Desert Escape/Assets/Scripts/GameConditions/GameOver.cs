using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    private PlayerController1 playerController; // Reference to the PlayerController script

    private void Start()
    {
        // Find and store the PlayerController component
        playerController = FindObjectOfType<PlayerController1>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Enemy" tag
        if (other.CompareTag("Enemy"))
        {
            // Load the "GameOver" scene
            SceneManager.LoadScene("GameOver");

            // Activate cursor using PlayerController
            playerController.ActivateCursor();
        }
    }

    // Function to restart the game
    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Function to go back to the map scene
    public void GoToMapScene()
    {
        // Load the map scene by index
        SceneManager.LoadScene("Map");
    }

    // Function to exit the game
    public void ExitGame()
    {
        // Quit the application
        Debug.Log("exit!");
        Application.Quit();
    }
}




