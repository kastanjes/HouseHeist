using UnityEngine;

public class EnemyRoaming : MonoBehaviour
{
    public float speed = 2f;                       // Movement speed of the enemy
    public float minPauseTime = 1f;                // Minimum time for a pause
    public float maxPauseTime = 3f;                // Maximum time for a pause
    public float minMoveTime = 2f;                 // Minimum time to move
    public float maxMoveTime = 5f;                 // Maximum time to move

    private Vector2 direction;                     // Current movement direction
    private float stateTimer;                      // Timer for the current state
    private bool isPaused;                         // Determines if the enemy is paused
    private Animator animator;                     // Reference to the Animator component

    void Start()
    {
        animator = GetComponent<Animator>();       // Get the Animator component
        ChooseNewDirection();
        SetNextState();                            // Set initial state (either move or pause)
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;

        if (isPaused)
        {
            // If paused, wait until the timer runs out
            if (stateTimer <= 0)
            {
                ChooseNewDirection();               // Set a new direction
                SetNextState();                    // Switch to the move state
            }
        }
        else
        {
            // Move the enemy in the chosen direction
            transform.Translate(direction * speed * Time.deltaTime);

            // Update animation parameters based on movement direction
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            // If moving, check if the timer has run out to pause
            if (stateTimer <= 0)
            {
                SetNextState();                    // Switch to the pause state
            }
        }
    }

    void ChooseNewDirection()
    {
        // Pick a random direction, only up, down, left, or right
        int directionIndex = Random.Range(0, 4); // Randomly pick one of four directions

        switch (directionIndex)
        {
            case 0:
                direction = Vector2.right;  // Move right
                break;
            case 1:
                direction = Vector2.left;   // Move left
                break;
            case 2:
                direction = Vector2.up;     // Move up
                break;
            case 3:
                direction = Vector2.down;   // Move down
                break;
        }
    }

    void SetNextState()
    {
        if (isPaused)
        {
            // If currently paused, start moving
            isPaused = false;
            stateTimer = Random.Range(minMoveTime, maxMoveTime);  // Random move duration
        }
        else
        {
            // If currently moving, start pausing
            isPaused = true;
            stateTimer = Random.Range(minPauseTime, maxPauseTime); // Random pause duration
            
            // Reset animation parameters to stop movement animation
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the enemy collides with a wall or obstacle
        if (collision.collider.CompareTag("Wall"))
        {
            // Choose a new direction if it hits a wall
            ChooseNewDirection();
        }
    }
}
