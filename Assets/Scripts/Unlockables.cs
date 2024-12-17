using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockables : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Unlock() 
    {
        AudioManager.instance.Play("Unlockable");

        Destroy(this.gameObject);
    }
}
