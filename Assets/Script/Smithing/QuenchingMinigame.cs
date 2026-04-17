using Microsoft.Unity.VisualStudio.Editor;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.UI;

public class QuenchingMinigame : MinigameBase
{
    [Header("Physics Settings")]
    [SerializeField] private float gravity = 80f;
    [SerializeField] private float upwardThrust = 100f;
    [SerializeField] private float maxVelocity = 60f;

    [Header("Target Settings")]
    [SerializeField] private float targetSize = 20f;
    [SerializeField] private float targetSpeed = 15f;
    [SerializeField] private float targetMaxHeight = 85f;
    [SerializeField] private float targetMinHeight = 15f;
    [SerializeField] private float requiredHoldTime = 3f;
    [SerializeField] private float timeLimitPerRound = 10f;

    [Header("Game Progression")]
    [SerializeField] private int maxRounds = 3;
    [SerializeField] private int maxFails = 3;

    [Header("UI References")]
    [SerializeField] private Slider playerPositionSlider;
    [SerializeField] private RectTransform targetZoneUI;
    [SerializeField] private Slider progressBarSlider;

    private float barPosition = 0f;
    private float barVelocity = 0f;

    private float targetPosition = 50f;
    private int targetDirection = 1;

    private float currentHoldTime = 0f;
    private float timeRemaining = 0f;

    private int currentRound = 0;
    private int currentFails = 0;

    public override void StartGame()
    {
        base.StartGame();
        score = 0;
        currentRound = 0;
        currentFails = 0;

        if(playerPositionSlider != null)
        {
            playerPositionSlider.minValue = 0f;
            playerPositionSlider.maxValue = 100f;
        }

        if(progressBarSlider != null)
        {
            progressBarSlider.minValue = 0f;
            progressBarSlider.maxValue = requiredHoldTime;
        }

        StartNewRound();
    }

    private void StartNewRound()
    {
        barPosition = 0f;
        barVelocity = 0f;
        currentHoldTime = 0f;
        timeRemaining = timeLimitPerRound;

        targetPosition = Random.Range(targetMinHeight, targetMaxHeight);

        targetDirection = Random.value < 0.5f ? -1 : 1;
    }

    private void Update()
    {
        if(!isPlaying) { return; }

        HandlePhysics();
        MoveTarget();
        CheckZone();
        HandleTimer();
    }

    private void HandlePhysics()
    {
        if(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            barVelocity += upwardThrust * Time.deltaTime;
        }
        else
        {
            barVelocity -= gravity * Time.deltaTime;
        }

        barVelocity = Mathf.Clamp(barVelocity, -maxVelocity, maxVelocity);
        barPosition += barVelocity * Time.deltaTime;

        if(barPosition <= 0f)
        {
            barPosition = 0f;
            barVelocity = 0f;
        }
        else if (barPosition >= 100f)
        {
            barPosition = 100f;
            barVelocity = 0f;
        }

        if(playerPositionSlider != null)
        {
            playerPositionSlider.value = barPosition;
        }
    }

    private void MoveTarget()
    {
        targetPosition += targetSpeed * targetDirection * Time.deltaTime;

        if (targetPosition >= targetMaxHeight)
        {
            targetPosition = targetMaxHeight;
            targetDirection = -1;
        }
        else if (targetPosition <= targetMinHeight)
        {
            targetPosition = targetMinHeight;
            targetDirection = 1;
        }

        UpdateTargetVisuals();
    }

    private void UpdateTargetVisuals()
    {
        if (targetZoneUI != null)
        {
            float normalizedPos = targetPosition / 100f;
            float normalizedSize = targetSize / 100f;

            targetZoneUI.anchorMin = new Vector2(0.1f, normalizedPos - (normalizedSize / 2f));
            targetZoneUI.anchorMax = new Vector2(0.9f, normalizedPos + (normalizedSize / 2f));
            targetZoneUI.offsetMin = Vector2.zero;
            targetZoneUI.offsetMax = Vector2.zero;
        }
    }

    private void CheckZone()
    {
        float distance = Mathf.Abs(barPosition - targetPosition);

        if(distance <= (targetSize / 2f))
        {
            currentHoldTime += Time.deltaTime;
        }
        else
        {
            currentHoldTime -= Time.deltaTime;
        }

        currentHoldTime = Mathf.Clamp(currentHoldTime, 0f, requiredHoldTime);

        if (progressBarSlider != null)
        {
            progressBarSlider.value = currentHoldTime;
        }

        if (currentHoldTime >= requiredHoldTime)
        {
            RoundWon();
        }
    }

    private void HandleTimer()
    {
        timeRemaining -= Time.deltaTime;

        if(timeRemaining <= 0)
        {
            FailAttemp();
        }
    }

    private void RoundWon()
    {
        Debug.Log("Round Won!");
        score += 33;
        currentRound++;

        if (currentRound >= maxRounds)
        {
            score = 100;
            Debug.Log($"Quenching Complete! Final Score: {score}");
            EndGame();
        }
        else
        {
            StartNewRound();
        }
    }

    private void FailAttemp()
    {
        currentFails++;
        Debug.Log($"Round Failed! Ran out of time. Fails: {currentFails}/{maxFails}");

        if (currentFails >= maxFails)
        {
            Debug.Log("Weapon Ruined! Quenching Failed.");
            EndGame();
        }
        else
        {
            StartNewRound();
        }
    }

    public int GetQuenchScore()
    {
        return (int)score;
    }
}
