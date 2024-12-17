using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    public Transform point;        // Position of the waypoint
    public Room roomToLight;       // Reference to the room to light up
    public float pauseDuration;    // Pause duration at this waypoint
}

public class GrandmaPath : MonoBehaviour
{
    public List<Waypoint> waypoints;   // List of waypoints for Grandma
    public float speed = 2f;           // Grandma's movement speed
    public Animator animator;          // Animator for Grandma animations

    private int currentWaypointIndex = 0; // Current waypoint index
    private bool isPaused = false;        // To track if Grandma is paused
    private bool pathFinished = false;    // To track when Grandma finishes her path

    void OnEnable()
    {
        pathFinished = false; // Reset path state
        currentWaypointIndex = 0; // Start from the first waypoint
        isPaused = false; // Ensure Grandma starts moving
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        while (!pathFinished && waypoints.Count > 0)
        {
            Waypoint waypoint = waypoints[currentWaypointIndex];

            // Move towards the waypoint
            yield return MoveToWaypoint(waypoint);

            // Pause at the waypoint and turn ON lights
            yield return PauseAtWaypoint(waypoint);

            // Turn OFF lights before leaving
            TurnOffRoomLights(waypoint);

            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;

            // If we've looped through all waypoints
            if (currentWaypointIndex == 0)
            {
                pathFinished = true;
            }
        }

        // Turn off Grandma after finishing path
        DeactivateGrandma();
    }

    IEnumerator MoveToWaypoint(Waypoint waypoint)
    {
        while (Vector2.Distance(transform.position, waypoint.point.position) > 0.1f)
        {
            Vector2 direction = (waypoint.point.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, waypoint.point.position, speed * Time.deltaTime);

            // Update animation to face movement direction
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);

            yield return null;
        }
    }

    IEnumerator PauseAtWaypoint(Waypoint waypoint)
    {
        isPaused = true;

        // Turn ON lights in the room
        if (waypoint.roomToLight != null)
        {
            waypoint.roomToLight.LightUpRoom();
            Debug.Log("Lights turned ON in room: " + waypoint.roomToLight.gameObject.name);

            // Check for players in the room
            if (waypoint.roomToLight.IsPlayerInRoom())
            {
                yield return StartCoroutine(ShootPlayer());
            }
        }

        animator.SetFloat("Horizontal", 0);
        animator.SetFloat("Vertical", 0);

        yield return new WaitForSeconds(waypoint.pauseDuration);

        isPaused = false;
    }

    void TurnOffRoomLights(Waypoint waypoint)
    {
        if (waypoint.roomToLight != null)
        {
            waypoint.roomToLight.TurnOffRoomLights();
            Debug.Log("Lights turned OFF in room: " + waypoint.roomToLight.gameObject.name);
        }
    }

    IEnumerator ShootPlayer()
    {
        animator.SetTrigger("Shoot");

        // Wait for the shooting animation to play 4 times (adjust as needed)
        yield return new WaitForSeconds(4f);

        FindObjectOfType<GameController>().TriggerGameOver();
        yield break;
    }

    void DeactivateGrandma()
    {
        Debug.Log("Grandma finished her path and is deactivating.");
        gameObject.SetActive(false);
    }
}
