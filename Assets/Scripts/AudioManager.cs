using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Master Volume")]
    public float masterVolume = 1f; // Default volume (1 = full, 0 = mute)

    [Header("Background Music")]
    public AudioClip backgroundMusicClip;
    public float musicVolume = 1f;
    public bool musicLoop = true;
    private AudioSource musicSource;

    [Header("Sound Effects")]
    public AudioClip barkClip;
    public float barkVolume = 1f;
    public bool barkLoop = true;
    private AudioSource barkSource;

    public AudioClip carDrivingClip;
    public float carDrivingVolume = 1f;
    public bool carDrivingLoop = true;
    private AudioSource carDrivingSource;

    public AudioClip victorySound;
    public float victorySoundVolume = 1f;
    private AudioSource victorySoundSource;

    public AudioClip doorOpenClip;
    public float doorOpenVolume = 1f;
    private AudioSource doorOpenSource;

    [Header("New Sound Effects")]
    public AudioClip gameOverSoundClip;
    public float gameOverSoundVolume = 1f;
    private AudioSource gameOverSoundSource;

    public AudioClip grandmaWalkingClip;
    public float grandmaWalkingVolume = 1f;
    public bool grandmaWalkingLoop = true;
    private AudioSource grandmaWalkingSource;

    public AudioClip collectablePickupClip;
    public float collectablePickupVolume = 1f;
    private AudioSource collectablePickupSource;
    public AudioClip moneyPickupClip;
public AudioClip biggerPickupClip;


    public AudioClip grandmaScreamClip; // NEW
    public float grandmaScreamVolume = 1f;
    private AudioSource grandmaScreamSource;

    public AudioClip shootingClip; // NEW
    public float shootingVolume = 1f;
    private AudioSource shootingSource;
    [Header("Hiding Sounds")]
    public AudioClip plantHideClip;
    public AudioClip closetHideClip;
    public AudioClip clothesHideClip;
    public AudioClip bathtubHideClip; // Add this for the bathtub sound
    [Header("Main Menu Music")]
    public AudioClip mainMenuMusicClip;
    public float mainMenuMusicVolume = 1f;
    private AudioSource mainMenuMusicSource;

        

    private AudioSource hidingSource;

    private AudioSource[] allAudioSources;

private void Start()
{
    CheckAndPlaySceneMusic();
}

    private void CheckAndPlaySceneMusic()
{
    if (GameObject.FindWithTag("MainMenu") != null) // Tag your Main Menu scene's main object
    {
        PlayMainMenuMusic();
    }
    else if (GameObject.FindWithTag("GameScene") != null) // Tag your Game Scene's main object
    {
        PlayBackgroundMusic();
    }
}


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize audio sources
        musicSource = AddAudioSource(backgroundMusicClip, musicVolume, musicLoop);
        barkSource = AddAudioSource(barkClip, barkVolume, barkLoop);
        carDrivingSource = AddAudioSource(carDrivingClip, carDrivingVolume, carDrivingLoop);
        victorySoundSource = AddAudioSource(victorySound, victorySoundVolume, false);
        doorOpenSource = AddAudioSource(doorOpenClip, doorOpenVolume, false);

        // Initialize new audio sources
        gameOverSoundSource = AddAudioSource(gameOverSoundClip, gameOverSoundVolume, false);
        grandmaWalkingSource = AddAudioSource(grandmaWalkingClip, grandmaWalkingVolume, grandmaWalkingLoop);
        collectablePickupSource = AddAudioSource(collectablePickupClip, collectablePickupVolume, false);
        grandmaScreamSource = AddAudioSource(grandmaScreamClip, grandmaScreamVolume, false); // NEW
        shootingSource = AddAudioSource(shootingClip, shootingVolume, false); // NEW
        // Initialize the hiding sound source
        hidingSource = AddAudioSource(null, 1f, false); // Volume and loop settings as needed
        
        // Initialize the main menu music
        mainMenuMusicSource = AddAudioSource(mainMenuMusicClip, mainMenuMusicVolume, true);
    }

private AudioSource AddAudioSource(AudioClip clip, float volume, bool loop)
{
    AudioSource source = gameObject.AddComponent<AudioSource>();
    source.clip = clip;
    source.volume = volume * masterVolume; // Apply master volume
    source.loop = loop;
    return source;
}


    public void PlaySound(AudioSource source, bool loop = false)
    {
        if (source != null && !source.isPlaying)
        {
            source.loop = loop;
            source.Play();
        }
    }

    public void StopSound(AudioSource source)
    {
        if (source != null && source.isPlaying)
        {
            source.Stop();
        }
    }

    // Background Music
    public void PlayBackgroundMusic() => PlaySound(musicSource, musicLoop);
    public void StopBackgroundMusic() => StopSound(musicSource);

    // Bark Sound
    public void PlayBarkSound() => PlaySound(barkSource, barkLoop);
    public void StopBarkSound() => StopSound(barkSource);

    // Car Driving Sound
public void PlayCarDrivingSound()
{
    if (carDrivingSource != null)
    {
        if (!carDrivingSource.isPlaying)
        {
            carDrivingSource.Play();
            Debug.Log("Car driving sound started.");
        }
    }
    else
    {
        Debug.LogError("Car driving sound source is missing!");
    }
}

public void StopCarDrivingSound()
{
    if (carDrivingSource != null)
    {
        carDrivingSource.Stop();
        Debug.Log("Car driving sound stopped.");
    }
}


public void ResetCarDrivingSource()
{
    if (carDrivingSource != null)
    {
        Destroy(carDrivingSource);
        carDrivingSource = null; // Ensure it doesn't persist
        Debug.Log("Car driving AudioSource destroyed.");
    }
}



    // Victory Sound
    public void PlayVictorySound() => PlaySound(victorySoundSource, false);
    public void StopVictorySound() => StopSound(victorySoundSource);

    // Door Open Sound
    public void PlayDoorOpen()
    {
        PlaySound(doorOpenSource, false);
        Debug.Log("Playing door open sound.");
    }

    // Game Over Sound
    public void PlayGameOverSound() => PlaySound(gameOverSoundSource, false);
    public void StopGameOverSound() => StopSound(gameOverSoundSource);

    // Grandma Walking Sound
    public void PlayGrandmaWalkingSound() => PlaySound(grandmaWalkingSource, grandmaWalkingLoop);
    public void StopGrandmaWalkingSound() => StopSound(grandmaWalkingSource);


    // Method to play collectable pickup sounds based on the provided clip
    public void PlayCollectablePickupSound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.volume = masterVolume; // Adjust volume using master volume
            source.loop = false;
            source.Play();

            // Destroy the AudioSource after the clip finishes playing
            Destroy(source, clip.length);
        }
        else
        {
            Debug.LogWarning("No audio clip provided for collectable pickup sound.");
        }
    }


    // Grandma Scream Sound
    public void PlayGrandmaScream() => PlaySound(grandmaScreamSource, false);

    // Shooting Sound
    public void PlayShootingSound() => PlaySound(shootingSource, false);

    public void PlayHidingSound(AudioClip clip)
    {
        if (clip != null)
        {
            hidingSource.clip = clip;
            hidingSource.Play();
        }
    }

        // Pause all audio sources
public void PauseAllSounds()
{
    allAudioSources = FindObjectsOfType<AudioSource>();
    Debug.Log($"Found {allAudioSources.Length} audio sources to pause.");
    foreach (AudioSource source in allAudioSources)
    {
        if (source.isPlaying)
        {
            source.Pause();
        }
    }
}


    // Resume all audio sources
public void ResumeAllSounds()
{
    if (allAudioSources != null)
    {
        Debug.Log($"Resuming {allAudioSources.Length} audio sources.");
        foreach (AudioSource source in allAudioSources)
        {
            source.UnPause();
        }
    }
}

public void PlayMainMenuMusic()
{
    if (mainMenuMusicSource != null && !mainMenuMusicSource.isPlaying)
    {
        mainMenuMusicSource.Play();
        Debug.Log("Main menu music started.");
    }
}

public void StopMainMenuMusic()
{
    if (mainMenuMusicSource != null && mainMenuMusicSource.isPlaying)
    {
        mainMenuMusicSource.Stop();
        Debug.Log("Main menu music stopped.");
    }
}

// Method to update the master volume
public void UpdateMasterVolume(float value)
{
    masterVolume = Mathf.Clamp(value, 0f, 1f); // Ensure volume is between 0 and 1

    // Update all audio sources with the new volume
    AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
    foreach (AudioSource source in audioSources)
    {
        source.volume = masterVolume; // Adjust volume based on masterVolume
    }

    Debug.Log($"Master volume updated to: {masterVolume}");
}


public void StopAllSounds()
{
    if (musicSource != null) StopSound(musicSource);
    if (carDrivingSource != null) StopSound(carDrivingSource);
    if (barkSource != null) StopSound(barkSource);
    if (victorySoundSource != null) StopSound(victorySoundSource);
    Debug.Log("All sounds stopped.");
}

public void ResetAllAudioSources()
{
    foreach (AudioSource source in FindObjectsOfType<AudioSource>())
    {
        source.Stop();
        Destroy(source);
    }
    Debug.Log("All AudioSources reset.");
}



}
