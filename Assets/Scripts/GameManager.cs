using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI player1ScoreText;  // Use TextMeshProUGUI for TMP components
    public TextMeshProUGUI player2ScoreText;

    private int player1Score = 0;
    private int player2Score = 0;

    private void Awake()
    {
        // Set up Singleton instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to add score to a specific player
    public void AddScore(int player, int score)
    {
        if (player == 1)
        {
            player1Score += score;
            player1ScoreText.text = "Player 1 Score: " + player1Score;
        }
        else if (player == 2)
        {
            player2Score += score;
            player2ScoreText.text = "Player 2 Score: " + player2Score;
        }
    }
}
