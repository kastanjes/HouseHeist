using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class FOVDirection : MonoBehaviour
{
    public EnemyRoaming enemyRoaming;
    public GameObject FOV;
    
    // Update is called once per frame
    void Update()
    {
        ChangeDirectionFOV();
    }   

public void ChangeDirectionFOV()
{
    if(enemyRoaming.direction == Vector2.right)
        {
            FOV.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if(enemyRoaming.direction == Vector2.left)
        {
            FOV.transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else if(enemyRoaming.direction == Vector2.up)
        {
            FOV.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        else if(enemyRoaming.direction == Vector2.down)
        {
            FOV.transform.eulerAngles = new Vector3(0, 0, 90);
        }
    }
}


