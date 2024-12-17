
using UnityEngine;

public class EnemyRoaming : MonoBehaviour
{
    public float speed = 2f;                       // Movement speed of the enemy
    public float chaseSpeed = 3.5f;                // Speed when chasing the player
    public float stopDistance = 1.5f;              // Distance to maintain from the player
    public float detectionDistance = 1f;           // Distance to detect walls/obstacles

    public float minPauseTime = 1f;                // Minimum pause duration
    public float maxPauseTime = 3f;                // Maximum pause duration
    public float minMoveTime = 2f;                 // Minimum move duration
    public float maxMoveTime = 5f;                 // Maximum move duration

    public Vector2 direction;                      // Current movement direction
    private float stateTimer;                      // Timer for the current state
    private bool isPaused;                         // Determines if the enemy is paused
    private bool isChasing = false;                // Determines if the enemy is in chasing mode
    private Transform playerToChase;               // Reference to the player being chased
    private Animator animator;                     // Reference to the Animator component
    private Rigidbody2D rb;                        // Reference to the Rigidbody2D component

    public Transform fovTransform;                 // Reference to the FOV object

    // Public property to expose isChasing
    public bool IsChasing
    {
        get { return isChasing; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ChooseNewDirection();
        SetNextState();
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Roam();
        }

        RotateFOV(); // Rotate the FOV in the direction of movement or chasing
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
            if (IsObstacleAhead())
            {
                ChooseNewDirection(); // Change direction if an obstacle is detected
                return;
            }

            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            if (stateTimer <= 0)
            {
                SetNextState();
            }
        }
    }

    bool IsObstacleAhead()
    {
        // Cast a ray in the current movement direction to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionDistance, LayerMask.GetMask("Default"));
        Debug.DrawRay(transform.position, direction * detectionDistance, Color.red);
        return hit.collider != null;
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

    void ChasePlayer()
    {
        if (playerToChase == null || playerToChase.GetComponent<PlayerMovement>().isHiding)
        {
            StopChasing();
            return;
        }

        // Calculate direction toward the player
        Vector2 directionToPlayer = (playerToChase.position - transform.position).normalized;

        // Calculate distance to player
        float distanceToPlayer = Vector2.Distance(playerToChase.position, transform.position);

        // Check stop distance
        if (distanceToPlayer > stopDistance)
        {
            // Move toward the player
            rb.MovePosition(rb.position + directionToPlayer * chaseSpeed * Time.fixedDeltaTime);

            animator.SetFloat("Horizontal", directionToPlayer.x);
            animator.SetFloat("Vertical", directionToPlayer.y);
        }
        else
        {
            // Stop moving but still face the player
            animator.SetFloat("Horizontal", directionToPlayer.x);
            animator.SetFloat("Vertical", directionToPlayer.y);
        }

        direction = directionToPlayer; // Set direction for FOV rotation
    }

    void StopChasing()
    {
        isChasing = false;
        playerToChase = null;
        ChooseNewDirection();
        SetNextState();
    }
  public void OnPlayerDetected(Transform player)
{
    PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
    if (playerMovement != null && !playerMovement.isHiding)
    {
        isChasing = true;
        playerToChase = player;
        playerMovement.OnPlayerDetected();
    }
}

public void OnPlayerLost()
{
    if (playerToChase != null)
    {
        PlayerMovement playerMovement = playerToChase.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.OnPlayerLost();
    }
    isChasing = false;
    playerToChase = null;
}


    void RotateFOV()
    {
        if (fovTransform != null)
        {
            // Rotate the FOV to face the current direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            fovTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
