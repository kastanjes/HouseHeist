using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;        // Base movement speed
    private float currentMoveSpeed;     // Current movement speed after applying weight

    public int playerID = 1;            // ID for distinguishing between Player1 and Player2
    public string playerName = "Player1";

    [Header("Weight System")]
    public float currentWeight = 0f;    // Current weight carried
    public float maxWeight = 100f;      // Maximum weight the player can carry
    public Slider weightBar;            // UI Slider to represent weight
    public TMP_Text weightText;         // UI Text to display "Weight: X/100"

    [Header("Score Settings")]
    public int score = 0;               // Player's score
    public TextMeshProUGUI scoreText;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Hiding Settings")]
    public bool isHiding = false;       // Player hiding status

    private Vector2 movement;

    void Start()
    {
        currentMoveSpeed = moveSpeed;
        UpdateWeightUI();
        UpdateScoreText();
    }

    void Update()
    {
        if (!isHiding)
        {
            // Player movement input
            if (playerID == 1)
            {
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");
            }
            else if (playerID == 2)
            {
                movement.x = Input.GetAxisRaw("Horizontal2");
                movement.y = Input.GetAxisRaw("Vertical2");
            }
        }
        else
        {
            movement = Vector2.zero; // Stop movement when hiding
        }

        // Update animations
        if (animator != null)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * currentMoveSpeed * Time.fixedDeltaTime);
    }

    public void AddToScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void AddWeight(float weight)
    {
        currentWeight += weight;
        currentWeight = Mathf.Clamp(currentWeight, 0, maxWeight);

        UpdateMovementSpeed();
        UpdateWeightUI();
    }

    void UpdateMovementSpeed()
    {
        // Reduce movement speed proportionally to weight carried (up to 50% slower)
        float weightRatio = currentWeight / maxWeight;
        currentMoveSpeed = moveSpeed * (1f - weightRatio * 0.5f);
    }

    void UpdateWeightUI()
    {
        // Update the slider value
        if (weightBar != null)
        {
            weightBar.value = currentWeight / maxWeight;
        }

        // Update the text above the slider
        if (weightText != null)
        {
            weightText.text = $"Weight: {currentWeight}/{maxWeight}";
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"{playerName}: {score}$";
        }
    }

    public void OnPlayerDetected()
    {
        Debug.Log($"Player {playerID} detected!");
        GameController.Instance.StartHideCountdown(playerID);
    }

    public void OnPlayerLost()
    {
        Debug.Log($"Player {playerID} lost!");
        isHiding = false;

        GameController.Instance.PlayerHid(playerID);
    }
}
