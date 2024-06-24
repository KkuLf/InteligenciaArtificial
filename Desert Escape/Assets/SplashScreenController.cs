using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenController : MonoBehaviour
{
    public float splashDuration = 3.0f; // Duración del splash screen

    void Start()
    {
        // Inicia la corrutina para cargar la siguiente escena después del tiempo especificado
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        // Espera el tiempo especificado
        yield return new WaitForSeconds(splashDuration);

        // Carga la siguiente escena (asume que la siguiente escena está en el índice 1)
        SceneManager.LoadScene(1);
    }
}
