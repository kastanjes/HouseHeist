using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealables : MonoBehaviour
{
    public float weightInKg = 1.0f; //Weight of the stealable
    public float valueInUSD = 1.0f; //Dollar Value of the stealable. can be adjusted to 10.f

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void Steal(PlayerController stolenByPlayer) 
    {
        Destroy(this.gameObject);

        stolenByPlayer.totalWeightInKg += weightInKg;

        int numberOfStealables = FindObjectsOfType<Stealables>().Length;

        UIController ui = FindObjectOfType<UIController>();

        ui.AddUSD(valueInUSD);
    }
}
