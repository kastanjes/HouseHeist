using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject grandmaPrefab;          // Reference to Grandma GameObject (inactive by default)
    public float detectionTimeThreshold = 5f; // Time before activating Grandma

    private float totalDetectionTime = 0f;    // Accumulated detection time
    private bool grandmaActivated = false;    // Flag to ensure Grandma activates only once

    void Start()
    {
        if (grandmaPrefab != null)
        {
            grandmaPrefab.SetActive(false); // Ensure Grandma starts inactive
        }
    }

    void Update()
    {
        if (EnemyIsChasing() && !grandmaActivated)
        {
            totalDetectionTime += Time.deltaTime;

            if (totalDetectionTime >= detectionTimeThreshold)
            {
                ActivateGrandma();
            }
        }
        else if (!EnemyIsChasing())
        {
            // Reset timer if no enemies are chasing
            totalDetectionTime = 0f;
        }
    }

    bool EnemyIsChasing()
    {
        // Check if any enemy is chasing the player
        EnemyRoaming[] enemies = FindObjectsOfType<EnemyRoaming>();
        foreach (EnemyRoaming enemy in enemies)
        {
            if (enemy.IsChasing)
            {
                return true;
            }
        }
        return false;
    }

public void ActivateGrandma()
{
    if (grandmaPrefab != null && !grandmaPrefab.activeSelf)
    {
        grandmaPrefab.SetActive(true);
        Debug.Log("Grandma has been activated!"); // Confirm Grandma is activated

        GameController.Instance.ShowGrandmaText();
    }
}





    public void DeactivateGrandma()
    {
        if (grandmaPrefab != null)
        {
            grandmaPrefab.SetActive(false);
            grandmaActivated = false; // Allow reactivation next time
            totalDetectionTime = 0f;  // Reset detection timer
            Debug.Log("Grandma deactivated.");
        }
    }
}
