using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// from https://www.youtube.com/watch?v=UHnOW-OimLQ&ab_channel=Garnet
public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections = new List<Node>(); // Ensure its initialized

    public float gScore;
    public float hScore;

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        if (connections != null && connections.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] != null) // ensure the connection is not null
                {
                    Gizmos.DrawLine(transform.position, connections[i].transform.position);
                }
                
            }
        }
    }
}
