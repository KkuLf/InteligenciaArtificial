using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            // Load the "Win" scene
            SceneManager.LoadScene("Win");

            Cursor.visible = true;
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
        Application.Quit();
    }
}

