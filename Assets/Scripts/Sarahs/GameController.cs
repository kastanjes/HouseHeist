using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Game Timer")]
    public float gameTime = 60f;
    private float currentTime;

    [Header("UI Elements")]
    public GameObject gameOverUI;
    public Button tryAgainButton;
    public Button menuButton;
    public TMP_Text timerText;

    [Header("Win Screen UI")]
    public GameObject winTextUI;            // Canvas with "You Win!"
    public TMP_Text winTextTMP;             // TextMeshPro for total score
    public GameObject vanGameObject;        // Van GameObject for animation

    [Header("Hide and Grandma UI")]
    public GameObject player1HideTextUI;
    public GameObject player2HideTextUI;
    public GameObject grandmaIsComingTextUI;

    private Coroutine hideCountdownCoroutine1;
    private Coroutine hideCountdownCoroutine2;
    private bool grandmaActivated = false;
    private bool winSequenceStarted = false;
    public bool isGameOver = false; // Tracks if the game is over

    [Header("Pause Screen")]
    public GameObject pauseUI;         // Canvas for Pause Screen
    public Button resumeButton;        // Resume Button
    public Button pauseMenuButton;     // Menu Button
    public GameObject pauseSprite;     // Sprite/Graphic for Pause Screen

    public bool isPaused = false;     // Tracks if the game is paused


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentTime = gameTime;
        gameOverUI.SetActive(false);
        winTextUI.SetActive(false);

        player1HideTextUI.SetActive(false);
        player2HideTextUI.SetActive(false);
        grandmaIsComingTextUI.SetActive(false);

        tryAgainButton.onClick.AddListener(RestartLevel);
        menuButton.onClick.AddListener(LoadMenu);

        resumeButton.onClick.AddListener(ResumeGame);
        pauseMenuButton.onClick.AddListener(LoadMenu);
        

    }


    void Update()
    {
        if (winSequenceStarted) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = $"Time Left: {Mathf.CeilToInt(currentTime)}s".Replace("\n", "");

        }
        else
        {
            TriggerGameOver();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

    }
public void TriggerWinSequence(int totalScore)
{
    if (winSequenceStarted) return;
    winSequenceStarted = true;

    // Freeze the timer
    currentTime = Mathf.CeilToInt(currentTime);
    timerText.text = "Time Left: " + currentTime + "s";

    // Show the win screen with the total score
    ShowWinScreen(totalScore);
}

IEnumerator HandleWinSequence(int totalScore)
{
    currentTime = Mathf.CeilToInt(currentTime); // Freeze the timer at its current value
    timerText.text = "Time Left: " + currentTime + "s";

    // Disable player visuals and controls
    foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
    {
        player.SetActive(false); // Hide player visuals
    }

    // Animate the van driving away
    float duration = 3f;
    Vector3 startPosition = vanGameObject.transform.position;
    Vector3 endPosition = startPosition + new Vector3(10f, 0, 0);

    float elapsed = 0f;
    while (elapsed < duration)
    {
        vanGameObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    // Show the win screen with the correct total score
    ShowWinScreen(totalScore);
}


    public void ShowWinScreen(int totalScore)
    {
        winTextUI.SetActive(true);
        winTextTMP.text = $"You Got Away!";


    }

    public void StartHideCountdown(int playerID)
    {
        if (playerID == 1 && hideCountdownCoroutine1 == null)
            hideCountdownCoroutine1 = StartCoroutine(HideCountdown(player1HideTextUI, 1));

        if (playerID == 2 && hideCountdownCoroutine2 == null)
            hideCountdownCoroutine2 = StartCoroutine(HideCountdown(player2HideTextUI, 2));
    }

// Method to handle the hide countdown
IEnumerator HideCountdown(GameObject hideText, int playerID)
{
    hideText.SetActive(true); // Show the hide text
    int countdown = 5; // Start countdown at 5 seconds

    while (countdown > 0)
    {
        // If player hides, stop the countdown and hide the text
        if (PlayerHid(playerID))
        {
            Debug.Log($"Player {playerID} hid successfully. Countdown stopped.");
            hideText.SetActive(false); // Hide the hide text
            ResetCoroutine(playerID); // Reset the coroutine reference
            yield break; // Exit the coroutine
        }

        // Update the text to show the current countdown value
        TMP_Text textComponent = hideText.GetComponent<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = $"Hide! {countdown}s";
        }
        else
        {
            Debug.LogError("Hide text GameObject does not have a TMP_Text component.");
        }

        yield return new WaitForSeconds(1f); // Wait for 1 second
        countdown--; // Decrease the countdown
    }

    // If the countdown finishes without the player hiding
    Debug.Log($"Player {playerID} failed to hide. Activating Grandma.");
    hideText.SetActive(false); // Hide the hide text
    ActivateGrandma(); // Trigger Grandma activation
    ResetCoroutine(playerID); // Reset the coroutine reference
}

// Method to activate Grandma and show the Grandma text
void ActivateGrandma()
{
    if (!grandmaActivated) // Only activate Grandma if she isn’t already active
    {
        grandmaActivated = true; // Set Grandma as active

        // Hide all hide texts to ensure only Grandma's text is visible
        if (player1HideTextUI != null) player1HideTextUI.SetActive(false);
        if (player2HideTextUI != null) player2HideTextUI.SetActive(false);

        // Activate Grandma through the EnemyManager
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.ActivateGrandma();
        }

        // Show Grandma’s text for a short duration
        if (grandmaIsComingTextUI != null)
        {
            StartCoroutine(ShowGrandmaTextRoutine());
        }
        else
        {
            Debug.LogError("Grandma text UI reference is missing!");
        }
    }
}

// Coroutine to show Grandma’s text for a set duration
private IEnumerator ShowGrandmaTextRoutine()
{
    grandmaIsComingTextUI.SetActive(true); // Show the Grandma text
    yield return new WaitForSeconds(3f); // Wait for 3 seconds
    grandmaIsComingTextUI.SetActive(false); // Hide the Grandma text
}


    public bool PlayerHid(int playerID)
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement player in players)
        {
            if (player.playerID == playerID && player.isHiding)
                return true;
        }
        return false;
    }

    public IEnumerator ShowGrandmaText()
    {
        grandmaIsComingTextUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        grandmaIsComingTextUI.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (winSequenceStarted) return;

        isGameOver = true; // Set game over state

        gameOverUI.SetActive(true);
        Time.timeScale = 0;

        Debug.Log("Stopping Bark sound in Game Over.");
        AudioManager.Instance.StopBarkSound(); // Stop the barking sound
    }


    void ResetCoroutine(int playerID)
    {
        if (playerID == 1) hideCountdownCoroutine1 = null;
        if (playerID == 2) hideCountdownCoroutine2 = null;
    }

public void RestartLevel()
{
    Time.timeScale = 1;

    // Stop all sounds explicitly
    AudioManager.Instance.ResetCarDrivingSource();
    AudioManager.Instance.StopVictorySound();
    AudioManager.Instance.StopBarkSound();

    // Reload the scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    // Ensure background music starts again after the scene reload
    StartCoroutine(PlayBackgroundMusicAfterSceneReload());
}

private IEnumerator PlayBackgroundMusicAfterSceneReload()
{
    yield return new WaitForEndOfFrame(); // Wait for the scene to reload
    AudioManager.Instance.PlayBackgroundMusic();
}





    public void LoadMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    AudioManager.Instance.StopAllSounds();
        
    }

public void PauseGame()
{
    Debug.Log("PauseGame called");
    isPaused = true;
    Time.timeScale = 0; // Freeze the game
    pauseUI.SetActive(true);
    pauseSprite.SetActive(true);

    // Pause all sounds
    AudioManager.Instance.PauseAllSounds();

    // Stop the barking sound explicitly
    AudioManager.Instance.StopBarkSound();
}



public void ResumeGame()
{
    Debug.Log("ResumeGame called");
    isPaused = false;
    Time.timeScale = 1; // Resume the game
    pauseUI.SetActive(false);
    pauseSprite.SetActive(false);

    // Resume all sounds
    AudioManager.Instance.ResumeAllSounds();

    // Restart the barking sound if the enemy is still chasing
    EnemyRoaming[] enemies = FindObjectsOfType<EnemyRoaming>();
    foreach (EnemyRoaming enemy in enemies)
    {
        if (enemy.IsChasing)
        {
            Debug.Log("Restarting Bark sound for chasing enemy.");
            AudioManager.Instance.PlayBarkSound();
        }
    }
}
}