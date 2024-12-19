using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame ()
    {
        Debug.Log("QUIT!");
        Application.Quit();

    }

public void LoadGame()
{
    Debug.Log("Load Game triggered");
    Time.timeScale = 1; // Ensure the game is not paused
    AudioManager.Instance.PlayBackgroundMusic();
    UnityEngine.SceneManagement.SceneManager.LoadScene("Sarahs"); // Replace "MainGame" with your actual game scene name
}

}
