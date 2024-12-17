using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Room : MonoBehaviour
{
    [Header("Light Control")]
    public Light2D roomLight;          // Light2D component
    public LayerMask playerLayer;      // Layer for player detection
    [Header("Player Detection")]
    public Vector2 detectionAreaSize = new Vector2(5f, 5f); // Adjustable size for player detection
    public Vector2 detectionOffset = Vector2.zero; // Offset to center the detection box if needed

    private bool lightsOn = false;

    public void LightUpRoom()
    {
        if (!lightsOn && roomLight != null)
        {
            lightsOn = true;
            roomLight.enabled = true;
            Debug.Log("Room lights ON: " + gameObject.name);
        }
    }

    public void TurnOffRoomLights()
    {
        if (lightsOn && roomLight != null)
        {
            lightsOn = false;
            roomLight.enabled = false;
            Debug.Log("Room lights OFF: " + gameObject.name);
        }
    }

    public bool IsPlayerInRoom()
    {
        Vector2 center = (Vector2)transform.position + detectionOffset;

        Collider2D[] players = Physics2D.OverlapBoxAll(
            center, 
            detectionAreaSize, 
            0f, 
            playerLayer);

        foreach (Collider2D player in players)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null && !playerMovement.isHiding)
            {
                Debug.Log("Player detected in room: " + gameObject.name);
                return true;
            }
        }
        return false;
    }

    // Visualize detection area in the Editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (Vector2)transform.position + detectionOffset;
        Gizmos.DrawWireCube(center, detectionAreaSize);
    }
}
