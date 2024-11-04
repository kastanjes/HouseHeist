using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public UnityEngine.UI.Slider progress;
    public GameObject gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetProgress()
    {
        return progress.value;
    }
    public void SetProgress(float newProgress)
    {
        progress.value = newProgress;
    }

    public void ShowGameOver(bool enabled) 
    {
        gameOverText.SetActive(enabled);
    }
}
