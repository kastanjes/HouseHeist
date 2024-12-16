using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knirkeables : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Afspil knirkelyd
            if (AudioManager.instance != null)
            {
                AudioManager.instance.Play("Knirkable");
            }

            // Start coroutine for lyde og Grandmas reaktion
            StartCoroutine(HandleGrandmaAndSounds());
        }
    }

    IEnumerator HandleGrandmaAndSounds()
    {
        // Vent lidt for at afspille fodtrin efter knirkelyden
        yield return new WaitForSeconds(0.7f);

        // Afspil fodtrin
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Play("Footsteps");
        }

        // Ekstra ventetid før Grandma starter sin undersøgelse
        yield return new WaitForSeconds(2.0f); // Tilføj 2 ekstra sekunder

        // Start Grandmas undersøgelse
        GrandmaController grandmaController = FindObjectOfType<GrandmaController>();
        if (grandmaController != null)
        {
            grandmaController.StartInvestigation();
        }
    }
}
