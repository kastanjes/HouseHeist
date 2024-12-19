using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider; // Reference to the slider

    void Start()
    {
        // Initialize the slider value from the current master volume
        if (AudioManager.Instance != null)
        {
            volumeSlider.value = AudioManager.Instance.masterVolume;
        }

        // Add a listener to handle slider value changes
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    void UpdateVolume(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMasterVolume(value);
        }
    }
}
