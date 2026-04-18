using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgingMinigame : MinigameBase
{
    [Header("Game Setting")]
    [SerializeField] private float RotationSpeed = 180f;
    [SerializeField] private int maxRounds = 3;
    [SerializeField] private int maxFails = 3;
    
    [Header("Minigame UI")]
    [SerializeField] private RectTransform needleTransform;
    [SerializeField] private RectTransform targetZoneTransform;

    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI currentA;
    [SerializeField] private TextMeshProUGUI SMax;
    [SerializeField] private TextMeshProUGUI SMin;
    [SerializeField] private TextMeshProUGUI PMax;
    [SerializeField] private TextMeshProUGUI PMin;

    private float successAngleMin = 60f;
    private float successAngleMax = 85f;
    private float perfectAngleMin = 70f;
    private float perfectAngleMax = 75f;

    private float currentAngle = 0f;
    private int currentRound = 0;
    private int currentFails = 0;
    private int needleDirection = 1;

    public override void StartGame()
    {
        base.StartGame();
        score = 0;
        currentRound = 0;
        currentFails = 0;
        needleDirection = 1;

        StartNewRound();
    }

    private void StartNewRound()
    {
        needleDirection *= -1;

        float randomAngle = Random.Range(0, 360);
        //currentAngle = 0f;

        successAngleMin = 0f;
        successAngleMax = 0f;
        perfectAngleMin = 0f;
        perfectAngleMax = 0f;

        successAngleMin = randomAngle;
        successAngleMax = successAngleMin + 72f;

        perfectAngleMin = successAngleMin + 27f;
        perfectAngleMax = perfectAngleMin + 18f;

        if (targetZoneTransform != null)
        {
            targetZoneTransform.localRotation = Quaternion.Euler(0, 0, -randomAngle);
        }
    }

    private void Update()
    {
        if (!isPlaying) { return; }

        MoveNeedle();
        HandleInput();

        AngleDebug();
    }

    private void MoveNeedle()
    {
        if (isPaused) { return; }

        currentAngle += RotationSpeed * needleDirection * Time.deltaTime;

        if(currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }
        if (currentAngle < 0f)
        {
            currentAngle += 360f;
        }
        
        if(needleTransform != null)
        {
            needleTransform.localRotation = Quaternion.Euler(0, 0, -currentAngle);
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            CheckHit();
        }
    }

    private void CheckHit()
    {
        if (IsAngleInZone(currentAngle, perfectAngleMin, perfectAngleMax))
        {
            Debug.Log("PERFECT Strike!");
            score += 34;
            currentRound++;
            StartNewRound();
        }
        
        else if (IsAngleInZone(currentAngle, successAngleMin, successAngleMax))
        {
            Debug.Log("Good Strike.");
            score += 20;
            currentRound++;
            StartNewRound();
        }

        else
        {
            Debug.Log("Missed!");
            currentFails++;

            if (currentFails >= maxFails)
            {
                Debug.Log("Weapon Ruined! Forging Failed.");
                EndGame();
                return;
            }
        }

        if (currentRound >= maxRounds)
        {
            score = Mathf.Clamp(score, 0, 100);
            Debug.Log($"Forging Complete! Final Score: {score}");
            EndGame();
        }
    }

    private bool IsAngleInZone(float current, float min, float max)
    {
        if (max > 360f) // Range wraps around
        {
            return current >= min || current <= max - 360f;
        }
        return current >= min && current <= max;
    }

    public int GetForgeScore()
    {
        return (int)score;
    }

    private void AngleDebug()
    {
        currentA.text = $"Current Angle: {currentAngle:F1}";
        SMax.text = $"Success Max: {successAngleMax:F1}";
        SMin.text = $"Success Min: {successAngleMin:F1}";
        PMax.text = $"Perfect Max: {perfectAngleMax:F1}";
        PMin.text = $"Perfect Min: {perfectAngleMin:F1}";
    }

    // Dramatic pause
    private bool isPaused = false;
    private void DramaticPause()
    {
        StartCoroutine(PauseRoutine());
    }

    private IEnumerator PauseRoutine()
    {
        isPaused = true;
        yield return new WaitForSecondsRealtime(1f); // uses real time, ignores timeScale
        isPaused = false;
    }

}
