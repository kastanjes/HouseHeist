using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerNumber
    {
        PlayerOne,
        PlayerTwo,
    }
    public PlayerNumber playerNumber;
    [Range(0.001f, 0.1f)] public float speed = 0.01f;

    public GameObject playerGraphics;
    public GameObject playerLight;

    bool canPlayerMove = true;

    public float totalWeightInKg = 1.0f;

    public Vector3 targetPosition;

    float CalculateAndGetSpeed()
    {
        return speed * 1 / totalWeightInKg; // more weight makes you slower
    }

    void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (playerNumber == PlayerNumber.PlayerOne)
        {
            UpdatePlayerOne();
        }
        else if (playerNumber == PlayerNumber.PlayerTwo)
        {
            UpdatePlayerTwo();
        }
    }

    void UpdatePlayerOne()
    {
        if (canPlayerMove)
        {
            // Move player
            float horizontal = 0.0f;
            if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1.0f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1.0f;
            }

            float vertical = 0.0f;
            if (Input.GetKey(KeyCode.W))
            {
                vertical = 1.0f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                vertical = -1.0f;
            }

            Vector2 position = transform.position;
            position.x = position.x + CalculateAndGetSpeed() * horizontal;
            position.y = position.y + CalculateAndGetSpeed() * vertical;
            targetPosition = position;
        }

        // Player pickup and interaction
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void UpdatePlayerTwo()
    {
        if (canPlayerMove)
        {
            // Move player
            float horizontal = 0.0f;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                horizontal = -1.0f;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                horizontal = 1.0f;
            }

            float vertical = 0.0f;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                vertical = 1.0f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                vertical = -1.0f;
            }

            Vector2 position = transform.position;
            position.x = position.x + CalculateAndGetSpeed() * horizontal;
            position.y = position.y + CalculateAndGetSpeed() * vertical;
            targetPosition = position;
        }

        // Player pickup and interaction
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }
    }

    void FixedUpdate()
    {
        // Ensure the position updates correctly
        transform.position = targetPosition;

        // Lock rotation to default
        transform.rotation = Quaternion.identity;
    }

    void Interact()
    {
        // Player is already hidden; unhide them
        if (playerGraphics.activeSelf == false)
        {
            playerGraphics.SetActive(true);
            canPlayerMove = true;
            if (playerLight != null)
            {
                playerLight.SetActive(true);
            }
        }
        else
        {
            Stealables closestStealable = FindStealables();

            // Check if the player can hide in a nearby hideable
            HideableObject closestHideable = FindHideables();
            if (closestHideable != null && closestHideable.isPlayerInTrigger)
            {
                // Hide the player
                playerGraphics.SetActive(false);
                canPlayerMove = false;
                if (playerLight != null)
                {
                    playerLight.SetActive(false);
                }
            }
            else if (closestStealable != null)
            {
                closestStealable.Steal(this);
            }

            if (playerNumber == PlayerNumber.PlayerTwo)
            {
                Unlockables closestUnlockable = FindUnlockables();
                if (closestUnlockable != null)
                {
                    closestUnlockable.Unlock();
                }
            }
        }
    }

    HideableObject FindHideables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (var item in colliders)
        {
            HideableObject hideable = item.gameObject.GetComponent<HideableObject>();
            if (hideable != null && hideable.isPlayerInTrigger)
            {
                Debug.Log("Found hideable in trigger");
                return hideable;
            }
        }
        return null;
    }

    Stealables FindStealables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (var item in colliders)
        {
            Stealables stealable = item.gameObject.GetComponent<Stealables>();
            if (stealable != null)
            {
                float distance = Vector2.Distance(transform.position, stealable.gameObject.transform.position);
                if (distance < 1.1f)
                {
                    Debug.Log("Found stealable");
                    return stealable;
                }
            }
        }
        return null;
    }

    Unlockables FindUnlockables()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10);
        foreach (var item in colliders)
        {
            Unlockables unlockable = item.gameObject.GetComponent<Unlockables>();
            if (unlockable != null)
            {
                float distance = Vector2.Distance(transform.position, unlockable.gameObject.transform.position);
                if (distance < 1.1f)
                {
                    Debug.Log("Found unlockable");
                    return unlockable;
                }
            }
        }
        return null;
    }

    public void KillPlayer()
    {
        canPlayerMove = false;

        UIController ui = FindObjectOfType<UIController>();
        ui.ShowGameOver(true);
    }
}
