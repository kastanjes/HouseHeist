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

    public AudioClip grandmaScreamClip; // NEW
    public float grandmaScreamVolume = 1f;
    private AudioSource grandmaScreamSource;

    public AudioClip shootingClip; // NEW
    public float shootingVolume = 1f;
    private AudioSource shootingSource;

    void Start()
    {
        PlayBackgroundMusic(); // Ensure this is called
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
    }

    private AudioSource AddAudioSource(AudioClip clip, float volume, bool loop)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
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
    public void PlayCarDrivingSound() => PlaySound(carDrivingSource, carDrivingLoop);
    public void StopCarDrivingSound() => StopSound(carDrivingSource);

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

    // Collectable Pickup Sound
    public void PlayCollectablePickupSound() => PlaySound(collectablePickupSource, false);

    // Grandma Scream Sound
    public void PlayGrandmaScream() => PlaySound(grandmaScreamSource, false);

    // Shooting Sound
    public void PlayShootingSound() => PlaySound(shootingSource, false);
}
