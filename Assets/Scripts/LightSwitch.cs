using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject roomLight;

    internal void Switch(bool enabled)
    {
        // Tænd eller sluk lyset
        roomLight.SetActive(enabled);

        // Afspil lyd, hvis lyset tændes
        if (enabled && AudioManager.instance != null)
        {
            AudioManager.instance.Play("Lightswitch on");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
