using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    public bool isPlayerInTrigger = false;

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
    // en bool chekker true or false, og jeg vil gerne tjekke om player er inde i triggeren eller ej, hvilket vil sige om hideable object trigger er til eller ej
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
