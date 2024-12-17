using UnityEngine;
using TMPro;

public class HidingPlace : MonoBehaviour
{
    [Header("Floating Text Settings")]
    public GameObject floatingTextPrefab; // Prefab for the floating text
    private GameObject floatingTextInstance; // Instance of the floating text
    private Transform nearbyPlayer; // Reference to the player near the hiding place
    private bool isPlayerHiding = false; // Flag to check if a player is hiding
    private Transform hidingPlayer; // Reference to the player currently hiding

    [Header("Detection Settings")]
    public float detectionRadius = 2.0f; // Customizable detection radius

    [Header("Text Offset")]
    public Vector3 textOffset = new Vector3(0f, 1.5f, 0f); // Offset for text position

    [Header("Sorting Layer Settings")]
    public string hidingPlaceSortingLayer = "BehindHiding"; // Sorting layer for hiding place
    private string originalHidingPlaceSortingLayer; // To store the original sorting layer

    private Canvas floatingTextCanvas; // Separate canvas for floating text

    void Start()
    {
        // Instantiate floating text above the hiding place
        if (floatingTextPrefab != null)
        {
            floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);
            floatingTextCanvas = floatingTextInstance.GetComponentInChildren<Canvas>();

            if (floatingTextCanvas != null)
            {
                // Ensure floating text remains on the default sorting layer (above everything)
                floatingTextCanvas.sortingLayerName = "UI";
                floatingTextCanvas.sortingOrder = 100;
            }

            UpdateFloatingText(""); // Initialize with empty text
            floatingTextInstance.SetActive(false); // Hide text initially
        }
        else
        {
            Debug.LogError("FloatingTextPrefab is not assigned!");
        }
    }

    void Update()
    {
        nearbyPlayer = null;

        // Check for players near the hiding place
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= detectionRadius) // If the player is close
            {
                nearbyPlayer = player.transform;

                // Update the text based on hiding state
                if (isPlayerHiding && hidingPlayer == player.transform)
                {
                    UpdateFloatingText(player.name == "Player1" ? "E to unhide" : "Space to unhide");
                }
                else
                {
                    UpdateFloatingText(player.name == "Player1" ? "E to hide" : "Space to hide");
                }

                floatingTextInstance.SetActive(true);
                HandleInput(player);
                return;
            }
        }

        // Hide the floating text if no player is nearby
        if (floatingTextInstance != null)
        {
            floatingTextInstance.SetActive(false);
        }
    }

    void HandleInput(GameObject player)
    {
        if ((Input.GetKeyDown(KeyCode.E) && player.name == "Player1") || 
            (Input.GetKeyDown(KeyCode.Space) && player.name == "Player2"))
        {
            if (!isPlayerHiding) // Hide the player
            {
                HidePlayer(player.transform);
            }
            else if (hidingPlayer == player.transform) // Unhide the player
            {
                UnhidePlayer();
            }
        }
    }

    void HidePlayer(Transform player)
    {
        hidingPlayer = player;
        isPlayerHiding = true;

        // Move player to the hiding place position
        player.position = transform.position;

        // Set hiding place to "BehindHiding" sorting layer
        SpriteRenderer hidingPlaceRenderer = GetComponent<SpriteRenderer>();
        if (hidingPlaceRenderer != null)
        {
            originalHidingPlaceSortingLayer = hidingPlaceRenderer.sortingLayerName;
            hidingPlaceRenderer.sortingLayerName = hidingPlaceSortingLayer;
        }

        // Disable player movement
        player.GetComponent<PlayerMovement>().isHiding = true;

        Debug.Log($"{player.name} is hiding! Hiding place layer moved to {hidingPlaceSortingLayer}");
    }

    void UnhidePlayer()
    {
        if (hidingPlayer != null)
        {
            // Restore hiding place sorting layer
            SpriteRenderer hidingPlaceRenderer = GetComponent<SpriteRenderer>();
            if (hidingPlaceRenderer != null)
            {
                hidingPlaceRenderer.sortingLayerName = originalHidingPlaceSortingLayer;
            }

            // Enable player movement
            hidingPlayer.GetComponent<PlayerMovement>().isHiding = false;

            Debug.Log($"{hidingPlayer.name} is no longer hiding. Hiding place layer restored.");
        }

        hidingPlayer = null;
        isPlayerHiding = false;
    }

    void UpdateFloatingText(string message)
    {
        var textMesh = floatingTextInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = message;
            textMesh.alignment = TextAlignmentOptions.Center;
        }
    }

    // To visualize the detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
