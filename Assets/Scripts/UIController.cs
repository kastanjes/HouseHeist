using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public UnityEngine.UI.Slider progress;
    public GameObject gameOverText;
    public TMPro.TextMeshProUGUI progresText; //reference to the text showing progress


    public float currentAmountUSDCollected; // The value collected so far
    private float totalAmountAchieveableUSD;

    public PlayerController PlayerOne;
    public PlayerController PlayerTwo;
    public Slider WeightPlayerOne;
    public Slider WeightPlayerTwo;
    public TMPro.TextMeshProUGUI progresTextWeightPlayerOne;
    public TMPro.TextMeshProUGUI progresTextWeightPlayerTwo;

    private float totalAmountWeightAchieveableKg;

    // Start is called before the first frame update
    void Start()
    {
        Stealables[] stealables = FindObjectsOfType<Stealables>();
        totalAmountAchieveableUSD = 0;

        foreach (Stealables stealable in stealables)
        {
            totalAmountAchieveableUSD += stealable.valueInUSD;
            totalAmountWeightAchieveableKg += stealable.weightInKg;
        }


        UpdateUI(); // initialize the UI
    }

    private void UpdateUI()
    {
        SetProgress(currentAmountUSDCollected / totalAmountAchieveableUSD);

        // Update the progress text
        progresText.text = $"${currentAmountUSDCollected} / ${totalAmountAchieveableUSD}";

        progresTextWeightPlayerOne.text = $"{(PlayerOne.totalWeightInKg - 1)} kg / {totalAmountWeightAchieveableKg} kg";
        progresTextWeightPlayerTwo.text = $"{(PlayerTwo.totalWeightInKg - 1)} kg / {totalAmountWeightAchieveableKg} kg";

        SetWeightPlayerOne((PlayerOne.totalWeightInKg - 1) / totalAmountWeightAchieveableKg);
        SetWeightPlayerTwo((PlayerTwo.totalWeightInKg - 1) / totalAmountWeightAchieveableKg);
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

    public void SetWeightPlayerOne(float newWeight)
    {
        WeightPlayerOne.value = newWeight;
    }

    public void SetWeightPlayerTwo(float newWeight)
    {
        WeightPlayerTwo.value = newWeight;
    }

    public void AddUSD(float usd)
    {
        currentAmountUSDCollected += usd;
        SetProgress(currentAmountUSDCollected / totalAmountAchieveableUSD);
        UpdateUI();
    }

  
    public void ShowGameOver(bool enabled) 
    {
        gameOverText.SetActive(enabled);
    }
}
