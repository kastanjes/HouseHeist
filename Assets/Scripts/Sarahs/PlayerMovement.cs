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

    [Header("Door Interaction")]
    public float detectionRadius = 1.5f;   // Radius to detect nearby doors
    public LayerMask doorLayer;            // LayerMask to detect only door objects


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
        // Player 1 Movement
        if (playerID == 1)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }
        // Player 2 Movement & Door Interaction
        else if (playerID == 2)
        {
            movement.x = Input.GetAxisRaw("Horizontal2");
            movement.y = Input.GetAxisRaw("Vertical2");

            DetectDoors(); // Check for doors in range and show text

            if (Input.GetKeyDown(KeyCode.Space)) // Door Interaction Key
            {
                InteractWithDoor(); // Interact with the door
            }
        }
    }
    else
    {
        movement = Vector2.zero; // Stop movement when hiding
        HideDoorTexts(); // Hide text when the player moves away or hides
    }

    UpdateAnimations();
}

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * currentMoveSpeed * Time.fixedDeltaTime);
    }

private void InteractWithDoor()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, doorLayer);

    foreach (Collider2D hit in hits)
    {
        Door door = hit.GetComponent<Door>();
        if (door != null)
        {
            door.ShowInteractionText(); // Show the text when in range

            if (Input.GetKeyDown(KeyCode.Space)) // Door Interaction Key
            {
                door.ToggleDoor(); // Open/close the door
                AudioManager.Instance.PlayDoorOpen();
                Debug.Log("Door toggled by Player 2.");
                return;
            }
        }
        else
        {
            door.HideInteractionText(); // Hide text when interaction ends
        }
    }
}


void OnDrawGizmos()
{
    if (playerID == 2) // Only for Player 2
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}


private void DetectDoors()
{
    // Detect doors in a circle around the player
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, doorLayer);

    foreach (Collider2D hit in hits)
    {
        Door door = hit.GetComponent<Door>();
        if (door != null)
        {
            door.ShowInteractionText(); // Show the interaction text
        }
    }
}

private void HideDoorTexts()
{
    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, doorLayer);

    foreach (Collider2D hit in hits)
    {
        Door door = hit.GetComponent<Door>();
        if (door != null)
        {
            door.HideInteractionText(); // Hide the interaction text
        }
    }
}


    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
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
        if (weightBar != null)
        {
            weightBar.value = currentWeight / maxWeight;
        }

        if (weightText != null)
        {
            weightText.text = $"Weight: {currentWeight}/{maxWeight} kg";
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
