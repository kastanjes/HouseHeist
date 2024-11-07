using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knirkeables : MonoBehaviour
{
   
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Find grandma and start her walk sequence
        GrandmaController grandmaController = FindObjectOfType<GrandmaController>();
        if (grandmaController != null)
        {
            grandmaController.StartInvestigation();
        }
    }

}
