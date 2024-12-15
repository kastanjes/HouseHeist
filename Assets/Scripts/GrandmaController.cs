using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class GrandmaController : MonoBehaviour
{
    [Header("Grandma Options")]
    public float detectionRadius = 5f; // Radius of the detection cone
    public float detectionAngle = 45f; // Half-angle of the detection cone
    public LayerMask playerLayer; // Layer of the players
    public LayerMask obstacleLayer; // Layer for obstacles (e.g., walls)

    public float speed = 0.5f;

    public float Waittime = 1f;

    public GameObject GrandmaGraphics;

    [Header("Path Settings")]
    public GameObject LightSwitch_01;
    public GameObject LightSwitch_02;
    public GameObject LightSwitch_03;
    public GameObject LightSwitch_04;
    public GameObject EndStair;

    Node currentNode;
    public List<Node> path = new List<Node>();

    public Node lightSwitch01StartNode;
    public Node lightSwitch02StartNode;
    public Node lightSwitch03StartNode;
    public Node lightSwitch04StartNode;

    [SerializeField]
    private Node startNode; //store Grandma´s starting node
    private Vector3 startPosition; //Store the starting position

    public enum StateMachine
    {
        None,
        WalkToLight01,
        WalkToLight02,
        WalkToLight03,
        WalkToLight04,
        ReturnToStart,
    }

    public StateMachine currentState = StateMachine.WalkToLight01;


    bool waiting = false;

    void Start()
    {
        if (startNode == null) // Allow manual assignment from Inspector
        {
            startNode = AStarManager.instance.FindNearestNode(transform.position);
        }
        startPosition = transform.position;
        currentNode = startNode;
        currentState = StateMachine.None;
        Debug.Log("Grandma starting at node: " + startNode.name);

        GrandmaGraphics.SetActive(false);
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


    void FixedUpdate()
    {
        DetectPlayersInCone();

        // Only proceed if Grandma is not waiting and no path is left
        if (!waiting && path.Count == 0)
        {
            switch (currentState)
            {
                case StateMachine.WalkToLight01:
                    if (MoveToGameObject(lightSwitch01StartNode, LightSwitch_01)) 
                    {
                        Debug.Log("Reached LightSwitch_01.");
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.WalkToLight02, lightSwitch02StartNode));
                    }
                    break;

                case StateMachine.WalkToLight02:
                    if (MoveToGameObject(lightSwitch02StartNode, LightSwitch_02))
                    {
                        Debug.Log("Reached LightSwitch_02.");
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.WalkToLight03, lightSwitch03StartNode));
                    }
                    break;

                case StateMachine.WalkToLight03:
                    if (MoveToGameObject(lightSwitch03StartNode, LightSwitch_03))
                    {
                        Debug.Log("Reached LightSwitch_03.");
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.WalkToLight04, lightSwitch04StartNode));
                    }
                    break;

                case StateMachine.WalkToLight04:
                    if (MoveToGameObject(lightSwitch04StartNode, LightSwitch_04))
                    {
                        Debug.Log("Reached LightSwitch_04.");
                        StartCoroutine(WaitAndTurnOnLight(StateMachine.ReturnToStart, startNode));
                    }
                    break;

                case StateMachine.ReturnToStart:
                    if (MoveToGameObject(startNode, EndStair))
                    {
                        GrandmaGraphics.SetActive(false);
                        Debug.Log("Returned to Start Position.");
                        path.Clear(); // Clear the path
                        currentState = StateMachine.None; // Reset the state machine
                    }
                    break;


                default:
                    Debug.Log("Idle. Waiting for StartInvestigation.");
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
        yield return new WaitForSeconds(1); // Simulate waiting time

        // Turn on the light for the current room
        LightSwitch light = FindLightSwitches();
        if (light != null)
        {
            light.Switch(true);
            Debug.Log($"Turned on light at: {light.name}");
        }

        // Transition to the next state
        currentState = nextState;
        currentNode = nextNode;
        path.Clear(); // Clear the path before moving to the next state
        waiting = false;

        Debug.Log($"State updated to: {currentState}. Moving to next light switch.");
    }

    bool MoveToGameObject(Node startNode, GameObject stopAt)
    {
        Node[] startNodes = startNode.transform.parent.GetComponentsInChildren<Node>();
        Node endNode = stopAt != null ? startNodes[startNodes.Length - 1] : startNode;
        float distanceToStopGameObject = stopAt != null ? Vector2.Distance(transform.position, stopAt.transform.position) : 0;

        if (path.Count == 0 && currentNode != endNode)
        {
            path = AStarManager.instance.GeneratePath(startNode, endNode);
            Debug.Log($"Generating path to {(stopAt != null ? stopAt.name : "Start Position")}. Path count: {path.Count}");
        }
        else if (path.Count == 0 && stopAt != null && distanceToStopGameObject <= 1.0f)
        {
            Debug.Log($"Reached: {stopAt.name}");
            return true; // Destination reached
        }
        else if (path.Count == 0 && stopAt == null)
        {
            Debug.Log("Returned to Start Position.");
            return true; // Returned to start
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
            Debug.Log($"Following path. Next target: {path[0].name}");
            Vector3 targetPosition = path[0].transform.position;
            Vector3 direction = targetPosition - transform.position;

            // Move towards the next node
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if Grandma has reached the current node
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                Debug.Log($"Reached node: {path[0].name}");
                currentNode = path[0]; // Update the current node
                path.RemoveAt(0);     // Remove the node from the path list
            }
        }
        else
        {
            Debug.Log("No nodes left in path.");
        }
    }


    public void StartInvestigation() 
    {
        StartCoroutine(StartInvestigationCoroutine());
    }

    IEnumerator StartInvestigationCoroutine()
    { 
        yield return new WaitForSeconds(Waittime);

        currentState = StateMachine.WalkToLight01;
        GrandmaGraphics.SetActive(true);

    }
}

