using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // Play main menu music if this is the main menu scene
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }
    }

    public void PlayGame()
    {
        // Stop main menu music and load the game scene
        AudioManager.Instance.StopMainMenuMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void LoadGame()
    {
        Debug.Log("Load Game triggered");
        Time.timeScale = 1; // Ensure the game is not paused
        AudioManager.Instance.StopMainMenuMusic(); // Stop main menu music
        AudioManager.Instance.PlayBackgroundMusic(); // Start game background music
        SceneManager.LoadScene("Sarahs"); // Replace with your game scene name
    }
}
