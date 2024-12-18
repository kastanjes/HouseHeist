using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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

void Awake()
{
    // Singleton pattern to ensure only one instance exists
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

    // Add AudioSources for background music and sound effects
    musicSource = gameObject.AddComponent<AudioSource>();
    musicSource.clip = backgroundMusicClip;
    musicSource.volume = musicVolume;
    musicSource.loop = true;

    barkSource = gameObject.AddComponent<AudioSource>();
    barkSource.clip = barkClip;
    barkSource.volume = barkVolume;
    barkSource.loop = true; // Bark sound should loop

    carDrivingSource = gameObject.AddComponent<AudioSource>();
    carDrivingSource.clip = carDrivingClip;
    carDrivingSource.volume = carDrivingVolume;
    carDrivingSource.loop = true;

    victorySoundSource = gameObject.AddComponent<AudioSource>();
    victorySoundSource.clip = victorySound;
    victorySoundSource.volume = victorySoundVolume;
    victorySoundSource.loop = false; // Set looping to false for victory sound
}

    private AudioSource AddAudioSource(AudioClip clip, float volume, bool loop)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        return source;
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void PlayBarkSound()
    {
        if (!barkSource.isPlaying)
        {
            Debug.Log("Playing Bark sound");
            barkSource.Play();
        }
    }

    public void StopBarkSound()
    {
        if (barkSource.isPlaying)
        {
            Debug.Log("Stopping Bark sound");
            barkSource.Stop();
        }
    }

    public void PlayCarDrivingSound()
    {
        if (!carDrivingSource.isPlaying)
        {
            Debug.Log("Playing Car Driving sound");
            carDrivingSource.Play();
        }
    }

    public void StopCarDrivingSound()
    {
        if (carDrivingSource.isPlaying)
        {
            Debug.Log("Stopping Car Driving sound");
            carDrivingSource.Stop();
        }
    }

    public void PlayVictorySound()
    {
        if (!victorySoundSource.isPlaying) // Ensure it only plays once
        {
            Debug.Log("Victory Sound Loop State: " + victorySoundSource.loop);
            Debug.Log("Playing Victory sound");
            victorySoundSource.Play();
        }
    }

    public void StopVictorySound()
    {
        if (victorySoundSource.isPlaying)
        {
            Debug.Log("Stopping Victory sound");
            victorySoundSource.Stop();
        }
    }
}
