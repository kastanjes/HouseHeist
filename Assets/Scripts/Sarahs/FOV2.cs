using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class EnemyVisionCone : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewRadius = 5f;  // How far the enemy can see
    public float viewAngle = 90f;  // Angle of the vision cone
    public int rayCount = 50;      // Number of rays in the vision cone
    public LayerMask obstacleMask; // Mask for obstacles blocking vision
    public LayerMask playerMask;   // Mask for detecting players
    public LayerMask ignoreMask;   // Mask to exclude the enemy's own collider
    public EnemyRoaming enemyRoaming; // Reference to the enemy's roaming behavior

    private Mesh visionMesh;       // Mesh for the vision cone
    private MeshRenderer meshRenderer;

    private Transform detectedPlayer; // Reference to the currently detected player

    void Start()
    {
        // Set up the mesh for the vision cone
        visionMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = visionMesh;

        // Add a transparent red material
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = new Color(1f, 0f, 0f, 0.05f); // Transparent red
    }

    void Update()
    {
        AlignToEnemyDirection(); // Ensure the vision cone faces the enemy's movement direction
        DrawVisionCone();        // Dynamically draw the vision cone
        DetectPlayer();          // Check for players within the vision cone
    }

    void AlignToEnemyDirection()
    {
        if (enemyRoaming == null) return;

        Vector2 direction = enemyRoaming.direction;

        // Rotate the GameObject itself based on movement direction
        if (direction == Vector2.right)
            transform.rotation = Quaternion.Euler(0, 0, 0); // Facing right
        else if (direction == Vector2.left)
            transform.rotation = Quaternion.Euler(0, 0, 180); // Facing left
        else if (direction == Vector2.up)
            transform.rotation = Quaternion.Euler(0, 0, 90); // Facing up
        else if (direction == Vector2.down)
            transform.rotation = Quaternion.Euler(0, 0, -90); // Facing down
    }

    void DrawVisionCone()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero); // Origin of the cone (local space)

        float angleStep = viewAngle / (rayCount - 1);
        float startAngle = -viewAngle / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + i * angleStep;

            // Rotate the raycast directions properly using the enemy's current rotation
            Vector3 direction = DirectionFromAngle(angle, false);
            Vector3 hitPoint = CastRay(transform.position, direction);

            // Convert hit points to local space for the mesh
            vertices.Add(transform.InverseTransformPoint(hitPoint));

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        // Update the mesh
        visionMesh.Clear();
        visionMesh.vertices = vertices.ToArray();
        visionMesh.triangles = triangles.ToArray();
        visionMesh.RecalculateNormals();
    }

    void DetectPlayer()
    {
        detectedPlayer = null;

        // Perform raycasts in the vision cone
        float angleStep = viewAngle / (rayCount - 1);
        float startAngle = -viewAngle / 2;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector3 direction = DirectionFromAngle(angle, false);

            // Perform raycast excluding the enemy collider
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, viewRadius, (obstacleMask | playerMask) & ~ignoreMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                detectedPlayer = hit.collider.transform;
                enemyRoaming.OnPlayerDetected(detectedPlayer); // Notify EnemyRoaming
                return; // Stop checking after detecting the first player
            }
        }

        // If no player is detected, notify EnemyRoaming to stop chasing
        if (detectedPlayer == null)
        {
            enemyRoaming.OnPlayerLost();
        }
    }

Vector3 CastRay(Vector3 origin, Vector3 direction)
{
    // Combine obstacle and player masks, then exclude ignoreMask (e.g., enemy's own layer)
    LayerMask combinedMask = (obstacleMask | playerMask) & ~ignoreMask;

    // Perform the raycast
    RaycastHit2D hit = Physics2D.Raycast(origin, direction, viewRadius, combinedMask);
    if (hit.collider != null)
    {
        return hit.point; // Return hit point if any collider is detected
    }
    return origin + direction * viewRadius; // Return full range if no collider is hit
}


    Vector3 DirectionFromAngle(float angle, bool isGlobal)
    {
        // Calculate the direction vector based on the given angle
        if (!isGlobal)
        {
            angle += transform.eulerAngles.z; // Adjust by the GameObject's local rotation
        }
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
    }
}
