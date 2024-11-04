using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stealables : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Steal() 
    {
        Destroy(this.gameObject);

        int numberOfStealables = FindObjectsOfType<Stealables>().Length;

        UIController ui = FindObjectOfType<UIController>();

        float progressIncrement = 1f / numberOfStealables;
        ui.SetProgress(ui.GetProgress() + progressIncrement);
    }
}
