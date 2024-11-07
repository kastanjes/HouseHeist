using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealables : MonoBehaviour
{
    public float WeightInKg = 1.0f;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void Steal(PlayerController stolenByPlayer) 
    {
        Destroy(this.gameObject);

        stolenByPlayer.TotalWeightInKg += WeightInKg;

        int numberOfStealables = FindObjectsOfType<Stealables>().Length;

        UIController ui = FindObjectOfType<UIController>();

        float progressIncrement = 1f / numberOfStealables;
        ui.SetProgress(ui.GetProgress() + progressIncrement);
    }
}
