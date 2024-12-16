using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ToolSelectionManager : MonoBehaviour
{
    private int currentPlayer = 1; // Track whose turn it is (1 for Player 1, 2 for Player 2)

    public void SelectTool(string toolName)
    {
        if (currentPlayer == 1)
        {
            GameManager.instance.player1Tool = toolName; // Save Player 1's choice
            Debug.Log("Player 1 selected: " + toolName);
            currentPlayer = 2; // Switch to Player 2
            // Optionally, update the UI to prompt Player 2
            UpdateToolSelectionUI("Player 2, Choose Your Tool");
        }
        else if (currentPlayer == 2)
        {
            GameManager.instance.player2Tool = toolName; // Save Player 2's choice
            Debug.Log("Player 2 selected: " + toolName);
            // After both players choose, load the game scene
            SceneManager.LoadScene("MainGameScene"); // Replace with your game scene's name
        }
    }

    private void UpdateToolSelectionUI(string message)
    {
        // Assume you have a Text element in the UI to show messages to the players
        // Example: Find and update a Text component on the tool selection panel
        GameObject.Find("ToolSelectionMessage").GetComponent<TMPro.TextMeshProUGUI>().text = message;
    }
}

