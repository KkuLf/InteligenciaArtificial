using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public PlayerController playerController; // Reference to the PlayerController script

    private void Start()
    {
        // Find and store the PlayerController component
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            Debug.Log($"GameOver triggered by '{gameObject.name}' touching '{other.name}'");
            Trigger();
        }
    }

    // Reusable entry point so any script (not just this trigger) can end the game
    // without duplicating the scene-load + cursor logic.
    public static void Trigger()
    {
        SceneManager.LoadScene("GameOver");
        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) playerController.ActivateCursor();
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
