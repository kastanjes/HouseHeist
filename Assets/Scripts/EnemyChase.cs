using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public VisionCone visionCone;             // Reference to the VisionCone script
    public float chaseSpeed = 3f;             // Speed of the enemy when chasing
    private Transform playerTransform;        // Reference to the player's Transform
    private bool isChasing = false;           // Track whether the enemy is in chase mode

    void Start()
    {
        playerTransform = visionCone.playerTransform;
    }

    void Update()
    {
        if (visionCone.IsPlayerInVisionCone())
        {
            StartChasing();
        }
        
        if (isChasing)
        {
            ChasePlayer();
        }
    }

    private void StartChasing()
    {
        isChasing = true;
        Debug.Log("Player detected! Starting chase.");
    }

    private void ChasePlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.position += directionToPlayer * chaseSpeed * Time.deltaTime;
    }
}
