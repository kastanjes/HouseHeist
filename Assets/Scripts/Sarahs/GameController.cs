using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    }

    void Update()
    {
        if (winSequenceStarted) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = "Time Left: " + Mathf.CeilToInt(currentTime) + "s";
        }
        else
        {
            TriggerGameOver();
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

    IEnumerator HideCountdown(GameObject hideText, int playerID)
    {
        hideText.SetActive(true);
        int countdown = 5;

        while (countdown > 0)
        {
            if (PlayerHid(playerID))
            {
                hideText.SetActive(false);
                ResetCoroutine(playerID);
                yield break;
            }

            hideText.GetComponent<TMP_Text>().text = $"Hide! {countdown}s";
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        hideText.SetActive(false);
        ActivateGrandma();
    }

    void ActivateGrandma()
{
    if (!grandmaActivated)
    {
        grandmaActivated = true;
        player1HideTextUI.SetActive(false);
        player2HideTextUI.SetActive(false);

        // Find and activate Grandma using EnemyManager (ensure you have this script set up)
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager != null)
        {
            enemyManager.ActivateGrandma();
        }

        StartCoroutine(ShowGrandmaText());
    }
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

        gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }

    void ResetCoroutine(int playerID)
    {
        if (playerID == 1) hideCountdownCoroutine1 = null;
        if (playerID == 2) hideCountdownCoroutine2 = null;
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void LoadMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
