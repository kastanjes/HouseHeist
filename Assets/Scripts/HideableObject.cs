using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Tilføj denne using statement øverst, da vi bruger Array.Find()

public class HideableObject : MonoBehaviour
{
    public string hideSoundName; // Navnet på lyden, der skal afspilles
    public bool isPlayerInTrigger = false; // Holder styr på, om spilleren er i triggeren

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log("Player entered hideable trigger");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log("Player exited hideable trigger");
        }
    }

    void Update()
    {
        // Hvis spilleren er i triggeren og trykker på "E" eller "Space", kan de gemme sig
        if (isPlayerInTrigger && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            Debug.Log("Player is hiding in: " + gameObject.name);
            Hide();
        }
    }

    private void Hide()
    {
        // Tjek om lyden eksisterer, før den afspilles
        if (!string.IsNullOrEmpty(hideSoundName) && AudioManager.instance != null)
        {
            Sound soundToPlay = Array.Find(AudioManager.instance.sounds, sound => sound.name == hideSoundName);
            
            if (soundToPlay != null)
            {
                Debug.Log($"Afspiller lyd: {hideSoundName}");
                AudioManager.instance.Play(hideSoundName);
            }
            else
            {
                Debug.LogWarning($"Kunne ikke finde lyden {hideSoundName} i AudioManager");
            }
        }
        else
        {
            Debug.LogWarning($"Ingen lyd afspillet for {gameObject.name}. Enten mangler AudioManager, eller lydenavnet er forkert.");
        }

        Debug.Log("Player has successfully hidden in " + gameObject.name);
    }
}