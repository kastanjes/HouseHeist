using UnityEngine;

public class EnemyRoaming : MonoBehaviour
{
    public float speed = 2f;                       // Movement speed of the enemy
    public float chaseSpeed = 3.5f;                // Speed when chasing the player
    public float minPauseTime = 1f;                // Minimum time for a pause
    public float maxPauseTime = 3f;                // Maximum time for a pause
    public float minMoveTime = 2f;                 // Minimum time to move
    public float maxMoveTime = 5f;                 // Maximum time to move
    public float stopDistance = 1.5f;              // Minimum distance to maintain from the player

    public Vector2 direction;                     // Current movement direction
    private float stateTimer;                      // Timer for the current state
    private bool isPaused;                         // Determines if the enemy is paused
    private bool isChasing;                        // Determines if the enemy is in chasing mode
    private Transform playerToChase;              // Reference to the detected player
    private Animator animator;                     // Reference to the Animator component
    private Rigidbody2D rb;                        // Reference to the Rigidbody2D component

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();          // Get the Rigidbody2D component
        animator = GetComponent<Animator>();       // Get the Animator component
        ChooseNewDirection();
        SetNextState();                            // Set initial state (either move or pause)
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            ChasePlayer();                         // Chase the player
        }
        else
        {
            Roam();
        }
    }

    void Roam()
    {
        stateTimer -= Time.deltaTime;

        if (isPaused)
        {
            if (stateTimer <= 0)
            {
                ChooseNewDirection();
                SetNextState();
            }
        }
        else
        {
            // Move using Rigidbody2D to respect physics
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

            // Update animator parameters
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            if (stateTimer <= 0)
            {
                SetNextState();
            }
        }
    }

    void ChasePlayer()
    {
        if (playerToChase == null)
        {
            isChasing = false;
            SetNextState();
            return;
        }

        // Calculate direction to the player
        Vector2 directionToPlayer = (playerToChase.position - transform.position).normalized;

        // Check distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerToChase.position);

        if (distanceToPlayer > stopDistance)
        {
            // Move towards the player while maintaining physics
            rb.MovePosition(rb.position + directionToPlayer * chaseSpeed * Time.fixedDeltaTime);

            // Update animator parameters
            animator.SetFloat("Horizontal", directionToPlayer.x);
            animator.SetFloat("Vertical", directionToPlayer.y);
        }
        else
        {
            // Stop moving but continue facing the player
            animator.SetFloat("Horizontal", directionToPlayer.x);
            animator.SetFloat("Vertical", directionToPlayer.y);
        }
    }

    void ChooseNewDirection()
    {
        int directionIndex = Random.Range(0, 4);

        switch (directionIndex)
        {
            case 0:
                direction = Vector2.right;
                break;
            case 1:
                direction = Vector2.left;
                break;
            case 2:
                direction = Vector2.up;
                break;
            case 3:
                direction = Vector2.down;
                break;
        }
    }

    void SetNextState()
    {
        if (isPaused)
        {
            isPaused = false;
            stateTimer = Random.Range(minMoveTime, maxMoveTime);
        }
        else
        {
            isPaused = true;
            stateTimer = Random.Range(minPauseTime, maxPauseTime);

            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 0);
        }
    }

void OnCollisionEnter2D(Collision2D collision)
{
    // If the enemy collides with anything EXCEPT the "Player", choose a new direction
    if (!collision.collider.CompareTag("Player"))
    {
        ChooseNewDirection();
    }
}


    public void OnPlayerDetected(Transform player)
    {
        playerToChase = player;
        isChasing = true;
    }

    public void OnPlayerLost()
    {
        playerToChase = null;
        isChasing = false;
    }
}
