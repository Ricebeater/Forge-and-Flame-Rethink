using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class SmelthingMinigame : MinigameBase
{
    private void Start()
    {
        score = 0;
    }

    public void AddScore()
    {
        score++;
        Debug.Log("Smelthing Score :" + score);
    }

    public int GetSmeltScore()
    {
        return (int)score;
    }

    [Header("Value")]
    [SerializeField] private float requiredHeat = 100f;
    [SerializeField] private float currentHeat = 0f;
    [SerializeField] private float heatIncreaseRate = 10f;
    [SerializeField] private float heatDecreaseRate = 1f;
    private bool isMinigameActive = false;
    private bool isMinigameFinnished = false;

    [Header("Timer")]
    [SerializeField] private float smeltDuration = 6f;
    private float smeltTimer = 0f;
    private bool timerStarted = false;

    //Calculate Avarage Heat
    private float heatAccumulator = 0f;
    private int heatSampleCount = 0;
    private float averageHeat = 0f;

    [Header("Debug")]
    public bool isMiniGameActive;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI heatText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image HeatBar;
    [SerializeField] private GameObject smeltingUI;
    [SerializeField] private Image TimerCircle;


    public void StartMinigame()
    {
        isMinigameActive = true;
        isMinigameFinnished = false;
        timerStarted = false;
        smeltTimer = 0f;
        currentHeat = 0f;
        heatAccumulator = 0f;
        heatSampleCount = 0;
        averageHeat = 0f;
        Debug.Log("Smelting minigame started.");
    }

    public void EndMinigame()
    {
        isMinigameActive = false;
        isMinigameFinnished = false;
        timerStarted = false;
        smeltTimer = 0f;
        currentHeat = 0f;
        Debug.Log("Smelting minigame ended.");
    }

    private void Update()
    {
        isMiniGameActive = isMinigameActive;
        currentHeat = Mathf.Clamp(currentHeat, 0f, requiredHeat);
        HandleUI();

        if (!isMinigameActive) { return; }

        // first click
        if (!timerStarted)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                float firstClickBoost = 90f; // boost the first click, player only need to maintian click rate for good score

                timerStarted = true;
                currentHeat += heatIncreaseRate + firstClickBoost;
                Debug.Log("Smelting timer started!!!");
            }

            return;

        }

        smeltTimer += Time.deltaTime;

        if (smeltTimer > 0f) { currentHeat -= heatDecreaseRate * Time.deltaTime; }

        if (Mouse.current.leftButton.wasPressedThisFrame) { currentHeat += heatIncreaseRate; }

        heatAccumulator += currentHeat;
        heatSampleCount++;

        if (smeltTimer >= smeltDuration)
        {
            timerStarted = false;
            averageHeat = heatSampleCount > 0 ? heatAccumulator / heatSampleCount : 0f;
            isMinigameActive = false;
            isMinigameFinnished = true;


            Debug.Log($"Smelting done! Average heat: {averageHeat:F1}");
        }
    }

    public float CalculatedScore()
    {
        if (currentHeat >= requiredHeat)
        {
            return 100f;
        }

        return ((averageHeat / requiredHeat) * 100f);

    }

    private void HandleUI()
    {
        smeltingUI.SetActive(isMinigameActive);

        int heatDisplay = Mathf.FloorToInt(currentHeat);
        heatText.text = "Heat: " + heatDisplay;

        if (timerText != null)
        {
            if (timerStarted)
            {
                float remaing = smeltDuration - smeltTimer;
                timerText.text = "Time: " + remaing.ToString("F1") + " s";
            }
            else
            {
                timerText.text = isMinigameActive ? "Left click to start!" : "";
            }
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + CalculatedScore().ToString("F1");
            scoreText.gameObject.SetActive(isMinigameFinnished);
        }

        float heatPercent = currentHeat / requiredHeat;
    }
}
