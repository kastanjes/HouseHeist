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
        AudioManager.Instance.PlayGrandmaWalkingSound();

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

        // Update Horizontal parameter
        animator.SetFloat("Horizontal", direction.x);

        transform.position = Vector2.MoveTowards(transform.position, waypoint.point.position, speed * Time.deltaTime);
        yield return null;
    }

    // Stop movement, reset Horizontal to 0
    animator.SetFloat("Horizontal", 0);
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
        animator.SetFloat("Horizontal", 0);

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
    AudioManager.Instance.StopGrandmaWalkingSound();
    AudioManager.Instance.StopBackgroundMusic();
    AudioManager.Instance.PlayGrandmaScream();
    AudioManager.Instance.PlayShootingSound();



    // Repeat the shooting animation 4 times
    for (int i = 0; i < 4; i++)
    {
        animator.SetTrigger("Shoot"); // Trigger the animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length); 
        // Wait for the duration of the current animation to complete
    }

    // After all animations are finished, wait for 3 seconds before Game Over
    yield return new WaitForSeconds(3f);

    AudioManager.Instance.PlayGameOverSound();


    FindObjectOfType<GameController>().TriggerGameOver(); // Trigger Game Over

}




    void DeactivateGrandma()
    {
        AudioManager.Instance.StopGrandmaWalkingSound();

        Debug.Log("Grandma finished her path and is deactivating.");
        gameObject.SetActive(false);
    }
}
