using UnityEngine;
using TMPro;

public class Collectable : MonoBehaviour
{
    public int value = 10;               // Value of the collectable
    public float weight = 5f;            // Weight of the collectable

    public GameObject floatingTextPrefab; // Prefab for floating text
    private GameObject floatingTextInstance;
    private Transform nearbyPlayer;      // Tracks the player in range
    public Vector3 textOffset = new Vector3(0f, 1.5f, 0f); // Offset for text position

    void Start()
    {
        if (floatingTextPrefab != null)
        {
            floatingTextInstance = Instantiate(floatingTextPrefab, transform.position + textOffset, Quaternion.identity);
            UpdateFloatingText();
            floatingTextInstance.SetActive(false);
        }
    }

    void Update()
    {
        nearbyPlayer = null; // Reset nearby player each frame

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= 2.0f)
            {
                nearbyPlayer = player.transform;
                UpdateFloatingText();
                floatingTextInstance.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E) && player.name == "Player1")
                {
                    Collect(player.GetComponent<PlayerMovement>());
                }
                else if (Input.GetKeyDown(KeyCode.Space) && player.name == "Player2")
                {
                    Collect(player.GetComponent<PlayerMovement>());
                }
                return;
            }
        }

        if (floatingTextInstance != null)
        {
            floatingTextInstance.SetActive(false);
        }
    }

    void UpdateFloatingText()
    {
        if (floatingTextInstance != null)
        {
            TextMeshProUGUI textMesh = floatingTextInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                string keyPrompt = (nearbyPlayer != null && nearbyPlayer.name == "Player1") ? "E" : "Space";
                textMesh.text = $"{keyPrompt}\n{value}$ ({weight}kg)";
            }
        }
    }

    void Collect(PlayerMovement player)
    {
        if (player != null)
        {
            player.AddToScore(value);
            player.AddWeight(weight);
        }

        Destroy(floatingTextInstance);
        Destroy(gameObject);
    }
}
