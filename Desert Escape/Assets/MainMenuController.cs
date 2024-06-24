using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Método para el botón de Play
    public void PlayGame()
    {
        // Asume que la pantalla de Map es la escena con el índice 1
        SceneManager.LoadScene("Map");
    }

    // Método para el botón de Quit
    public void QuitGame()
    {
        // Sale del juego. No funciona en el editor de Unity, pero funcionará en una build.
        Application.Quit();

        // Para verificar que el botón funciona en el editor, puedes usar Debug.Log:
        Debug.Log("Game is exiting");
    }
}