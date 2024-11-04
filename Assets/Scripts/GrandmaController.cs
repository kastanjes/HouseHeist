using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GrandmaController : MonoBehaviour
{

    public float detectionRadius = 5f; // Radius of the detection cone
    public float detectionAngle = 45f; // Half-angle of the detection cone
    public LayerMask playerLayer; // Layer of the players
    public LayerMask obstacleLayer; // Layer for obstacles (e.g., walls)

    public float speed = 0.5f;


    public GameObject LightSwitch_01;
    public GameObject LightSwitch_02;
    public GameObject LightSwitch_03;
    public GameObject LightSwitch_04;

    Node currentNode;
    public List<Node> path = new List<Node>();

    public Node lightSwitch01StartNode;
    public Node lightSwitch02StartNode;
    public Node lightSwitch03StartNode;
    public Node lightSwitch04StartNode;

    public enum StateMachine
    {
        None,
        WalkToLight01,
        WalkToLight02,
        WalkToLight03,
        WalkToLight04,
    }

    public StateMachine currentState = StateMachine.WalkToLight01;


    bool waiting = false;

    void Start()
    {
        currentState = StateMachine.WalkToLight01;
    }

    void DetectPlayersInCone()
    {
        // Find all players within the detection radius
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);

        foreach (Collider2D player in playersInRange)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);

            // Check if the player is within the cone angle
            if (angleToPlayer < detectionAngle)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

                // Use a Raycast to check for obstacles between the enemy and the player
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, obstacleLayer | playerLayer);

                // If the ray hits the player directly (no walls in between), detect the player
                if (hit && hit.collider.gameObject == player.gameObject)
                {
                    PlayerController playerController = player.gameObject.GetComponent<PlayerController>();

                    if (playerController != null)
                    {
                        playerController.KillPlayer();
                    }
                    Debug.Log("Player detected in cone with clear line of sight!");
                }
            }
        }
    }

    // Optional: Visualize the detection cone in the Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftBoundary = Quaternion.Euler(0, 0, detectionAngle) * transform.right;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, -detectionAngle) * transform.right;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRadius);
    }


    void Update()
    {
        DetectPlayersInCone();

        // Check if Grandma is waiting at a light switch; if not, continue
        if (!waiting && path.Count == 0)
        {
            switch (currentState)
            {
                case StateMachine.WalkToLight01:
                    if (MoveToLightSwitch(lightSwitch01StartNode, LightSwitch_01))
                    {
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.WalkToLight02, lightSwitch02StartNode));
                    }
                    break;
                case StateMachine.WalkToLight02:
                    if (MoveToLightSwitch(lightSwitch02StartNode, LightSwitch_02))
                    {
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.WalkToLight03, lightSwitch03StartNode));
                    }
                    break;
                case StateMachine.WalkToLight03:
                    if (MoveToLightSwitch(lightSwitch03StartNode, LightSwitch_03))
                    {
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.WalkToLight04, lightSwitch04StartNode));
                    } 
                    break;
                case StateMachine.WalkToLight04:
                    if (MoveToLightSwitch(lightSwitch04StartNode, LightSwitch_04))
                    {
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.None, null));
                    }
                    break;
            }
        }

        if (path.Count > 0)
        {
            FollowPath();
        }
    }

    IEnumerator WaitAndTurnOnLight(StateMachine nextState, Node nextNode)
    {
        waiting = true;
        yield return new WaitForSeconds(1); // Wait 1 second before turning on the light

        // Turn on the light for the room
        LightSwitch light = FindLightSwitches();
        if (light != null)
        {
            light.Switch(true);
            Debug.Log("Turned on light at: " + light.name);
        }

        // Proceed to the next light switch
        currentState = nextState;
        currentNode = nextNode;
        path.Clear();
        waiting = false;
    }

    bool MoveToLightSwitch(Node startNode, GameObject stopAt)
    {
        Node[] startNodes = startNode.transform.parent.GetComponentsInChildren<Node>();
        float distanceToStopGameObject = Vector2.Distance(transform.position, stopAt.transform.position);
        Node endNode = startNodes[startNodes.Length - 1];

        // Only generate path if it hasn't been generated yet
        if (path.Count == 0 && distanceToStopGameObject > 1.0f)
        {
            path = AStarManager.instance.GeneratePath(startNode, endNode);
            Debug.Log("Generating path to light switch: " + stopAt.name);
        }
        else if (path.Count == 0 && distanceToStopGameObject <= 1.0f)
        {
            Debug.Log("Reached light switch: " + stopAt.name);
            return true; // Grandma has reached the light switch
        }
        return false;
    }

    LightSwitch FindLightSwitches()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (var item in colliders)
        {
            LightSwitch lightSwitch = item.gameObject.GetComponent<LightSwitch>();
            if (lightSwitch != null)
            {
                float distance = Vector2.Distance(transform.position, lightSwitch.gameObject.transform.position);
                if (distance < 1.1f)
                {
                    Debug.Log("Found lightSwitch: " + lightSwitch.name);
                    return lightSwitch;
                }
            }
        }
        return null;
    }

    public void FollowPath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            Vector3 targetPosition = new Vector3(path[x].transform.position.x, path[x].transform.position.y, transform.position.z);
            Vector3 direction = targetPosition - transform.position;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), 30.0f);

            if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
    }
}

