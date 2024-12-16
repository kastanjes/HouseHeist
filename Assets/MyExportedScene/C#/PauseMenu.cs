using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; 
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);        
        Time.timeScale = 0f;             
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);      
        Time.timeScale = 1f;             
        isPaused = false;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;             
        Debug.Log("Quitting the game...");
        Application.Quit();             
       
    }
}

