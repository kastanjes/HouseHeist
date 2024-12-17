using System.Collections;
using UnityEngine;
using TMPro;

public class VanController : MonoBehaviour
{
    [Header("End Text Settings")]
    public GameObject player1EndTextPrefab; // Prefab for Player 1's end text
    public GameObject player2EndTextPrefab; // Prefab for Player 2's end text
    public Vector3 player1TextOffset;       // Adjustable position for Player 1 text
    public Vector3 player2TextOffset;       // Adjustable position for Player 2 text

    [Header("Van Settings")]
    public Transform vanTransform;          // Van Transform to drive away
    public float driveTime = 3f;            // Duration of van driving
    public float driveSpeed = 5f;           // Speed of the van
    public float activationRadius = 2.0f;   // Radius for players to activate end text

    private GameObject player1EndTextInstance;
    private GameObject player2EndTextInstance;

    private bool player1Ready = false;
    private bool player2Ready = false;

    private CameraControl cameraControl;    // Reference to the CameraControl script

    void Start()
    {
        // Instantiate End Texts and position them using the offset
        if (player1EndTextPrefab != null)
        {
            player1EndTextInstance = Instantiate(player1EndTextPrefab, transform.position + player1TextOffset, Quaternion.identity);
            player1EndTextInstance.SetActive(false);
        }

        if (player2EndTextPrefab != null)
        {
            player2EndTextInstance = Instantiate(player2EndTextPrefab, transform.position + player2TextOffset, Quaternion.identity);
            player2EndTextInstance.SetActive(false);
        }

        // Get reference to the CameraControl script
        cameraControl = FindObjectOfType<CameraControl>();
    }

    void Update()
    {
        CheckPlayersInRange();
    }

    void CheckPlayersInRange()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= activationRadius)
            {
                if (player.name == "Player1" && !player1Ready)
                {
                    player1EndTextInstance.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        player1Ready = true;
                        player1EndTextInstance.SetActive(false);
                        Destroy(player); // Simulate entering the van
                    }
                }

                if (player.name == "Player2" && !player2Ready)
                {
                    player2EndTextInstance.SetActive(true);

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        player2Ready = true;
                        player2EndTextInstance.SetActive(false);
                        Destroy(player); // Simulate entering the van
                    }
                }
            }
            else
            {
                // Hide text when players move out of range
                if (player.name == "Player1" && !player1Ready)
                    player1EndTextInstance.SetActive(false);
                if (player.name == "Player2" && !player2Ready)
                    player2EndTextInstance.SetActive(false);
            }
        }

        // If both players are ready, trigger the van to drive
        if (player1Ready && player2Ready)
        {
            // Update the CameraControl script to follow the van
            if (cameraControl != null)
            {
                cameraControl.m_Targets = new Transform[] { vanTransform };
            }

            StartCoroutine(DriveVanAway());
        }
    }
IEnumerator DriveVanAway()
{
    int totalScore = 0;

    // Collect scores from all players BEFORE they are destroyed
    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
    foreach (GameObject player in players)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            totalScore += movement.score; // Sum the scores
        }
    }

    // Trigger the win sequence with the correct total score
    GameController.Instance.TriggerWinSequence(totalScore);

    // Move the van after triggering the win
    float elapsedTime = 0f;
    while (elapsedTime < driveTime)
    {
        vanTransform.Translate(Vector3.right * driveSpeed * Time.deltaTime);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Destroy players AFTER calculating the score
    foreach (GameObject player in players)
    {
        Destroy(player);
    }
}




}
