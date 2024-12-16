using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public float delayBeforeLoading = 3f; // Time in seconds before switching scenes
    public string mainMenuSceneName = "MainMenu"; // Replace with the name of your main menu scene

    private void Start()
    {
        // Start the delayed load of the main menu scene
        Invoke("LoadMainMenu", delayBeforeLoading);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

