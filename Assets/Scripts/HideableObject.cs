using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    public string hideSoundName; // Navnet på lyden, der skal afspilles
    public float interactionRange = 2f; // Hvor langt spilleren kan være fra objektet for at interagere

    private Transform player; // Reference til spilleren

    void Start()
    {
        // Find spilleren (forudsat spilleren har tagget "Player")
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Spilleren blev ikke fundet! Sørg for, at spilleren har tagget 'Player'.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Beregn afstanden mellem spilleren og objektet
            float distanceToPlayer = Vector2.Distance(player.position, transform.position);

            // Hvis spilleren er inden for rækkevidde og trykker på en knap, kan de gemme sig
            if (distanceToPlayer <= interactionRange && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
            {
                Debug.Log($"Spilleren interagerer med: {gameObject.name}");
                Hide();
            }
        }
    }

    public void Hide()
    {
        Debug.Log($"Hide() kaldt på {gameObject.name}");

        // Afspil lyden, hvis den er korrekt sat op
        if (!string.IsNullOrEmpty(hideSoundName) && AudioManager.instance != null)
        {
            Debug.Log($"Afspiller lyd: {hideSoundName}");
            AudioManager.instance.Play(hideSoundName);
        }
        else
        {
            Debug.LogWarning($"Ingen lyd afspillet for {gameObject.name}. Enten mangler AudioManager, eller lydenavnet er forkert.");
        }

        Debug.Log("Spilleren skjuler sig i " + gameObject.name);
    }
}
