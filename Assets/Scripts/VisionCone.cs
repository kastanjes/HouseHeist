using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VisionCone : MonoBehaviour
{
    public float viewDistance = 5f;          // The maximum distance the enemy can "see"
    public float viewAngle = 90f;            // The angle of the cone
    public int resolution = 50;              // Number of segments in the cone
    public Transform enemyTransform;         // Reference to the enemy's Transform
    public Transform playerTransform;

    private Mesh mesh;
    private MeshFilter meshFilter;

    void Start()
    {
        // Set initial Z position and assign the mesh to the MeshFilter
        transform.position = new Vector3(transform.position.x, transform.position.y, -0.2f);
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    void Update()
    {
        // Update position and direction to align with the enemy
        AlignWithEnemy();

        // Generate the vision cone mesh
        GenerateVisionCone();
    }

    public bool IsPlayerInVisionCone()
{
    float angleStep = viewAngle / resolution;
    float startAngle = -viewAngle / 2;

    for (int i = 0; i <= resolution; i++)
    {
        float currentAngle = startAngle + (i * angleStep);
        Vector3 direction = Quaternion.Euler(0, 0, currentAngle) * enemyTransform.up;

        RaycastHit2D hit = Physics2D.Raycast(enemyTransform.position, direction, viewDistance);

        if (hit.collider != null && hit.collider.transform == playerTransform)
        {
            return true;  // Player is within vision cone and detected by the raycast
        }
    }
    return false;
}


    void AlignWithEnemy()
    {
        // Align the vision cone's position to the enemy's position
        transform.position = new Vector3(enemyTransform.position.x, enemyTransform.position.y, -0.2f);

        // Determine the direction based on enemy movement
        Vector2 enemyVelocity = enemyTransform.GetComponent<Rigidbody2D>().velocity;

        if (enemyVelocity.x > 0.1f)
        {
            // Facing Right
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log("Vision cone facing right");
        }
        else if (enemyVelocity.x < -0.1f)
        {
            // Facing Left
            transform.rotation = Quaternion.Euler(0, 0, 180);
            Debug.Log("Vision cone facing left");
        }
        else if (enemyVelocity.y > 0.1f)
        {
            // Facing Up
            transform.rotation = Quaternion.Euler(0, 0, 90);
            Debug.Log("Vision cone facing up");
        }
        else if (enemyVelocity.y < -0.1f)
        {
            // Facing Down
            transform.rotation = Quaternion.Euler(0, 0, 270);
            Debug.Log("Vision cone facing down");
        }
    }

    void GenerateVisionCone()
    {
        // Generate vertices and triangles for the cone
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Center of the cone (where the enemy is)
        vertices.Add(new Vector3(0, 0, 0.1f)); // Set Z to 0.1 to avoid z-fighting

        float angleIncrement = viewAngle / resolution;
        float startAngle = viewAngle / 2;

        for (int i = 0; i <= resolution; i++)
        {
            float angle = startAngle - i * angleIncrement;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
            vertices.Add(new Vector3(direction.x * viewDistance, direction.y * viewDistance, 0.1f));

            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }


        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        
    }
}
