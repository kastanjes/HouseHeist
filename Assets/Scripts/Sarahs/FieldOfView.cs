using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;


public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private float fov;
    private Vector3 origin;
    private float startingAngle;

    public static Vector3 GetVectorFromAngle(float angles) 
        {
            float angleRad = angles * (Mathf.PI/180f);
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }
    
    private void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        fov = 90f;
        origin = Vector3.zero;
    }

    private void Update()
    {
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 50f;


        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin; 


        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <=rayCount; i++)
        {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);


            if(raycastHit2D.collider == null)
            // No collission
            {
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            }
            else
            // Collision with object
            {
                vertex = raycastHit2D.point - (Vector2)transform.position;
            }
            vertices[vertexIndex] = vertex;

            if(i>0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex +=3;
            }
            
            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf. Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if( n <0) n += 360;
        return n;
    }


    public void SetOrigin(Vector3 origin) {
        this.origin = origin;
        transform.position = origin;
        Debug.Log("FOV Origin: " + origin);

    }

 public void SetAimDirection(Vector3 aimDirection)
{
    // Check the direction and set the starting angle accordingly
    if (aimDirection.y > 0.1f) // Moving up
    {
        startingAngle = 180f - fov / 2f; // 0 degrees points up in Unity's 2D space
    }
    else if (aimDirection.y < -0.1f) // Moving down
    {
        startingAngle = 0f - fov / 2f; // 180 degrees points down
    }
    else if (aimDirection.x > 0.1f) // Moving right
    {
        startingAngle = 90f - fov / 2f; // 90 degrees points right
    }
    else if (aimDirection.x < -0.1f) // Moving left
    {
        startingAngle = 270f - fov / 2f; // 270 degrees points left
    }
    else
    {
        Debug.LogWarning("No clear direction detected. Keeping the current FOV angle.");
    }
}


}



