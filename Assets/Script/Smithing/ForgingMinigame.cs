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
    [SerializeField] private GameObject endGameButton;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;
    [SerializeField] private Sprite heartBroken;
    [SerializeField] private Sprite heartFull;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform popUpPos;
    [SerializeField] private GameObject perfectHitFX;
    [SerializeField] private GameObject goodHitFX;
    [SerializeField] private GameObject missHitFX;
    [SerializeField] private RectTransform hotMetal;
    [SerializeField] private CanvasGroup hotness;

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

        if (endGameButton != null) { endGameButton.SetActive(false); }

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

        UpdateProgressBarUI();
        HandleHotMetalAnimation();
        UpdateHeart();

        AngleDebug();
    }

    private void MoveNeedle()
    {
        if (isPaused) { return; }
        if (currentRound >= maxRounds) { return; }
        if (currentFails >= maxFails) { return; }

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
            if (currentRound < maxRounds && currentFails < maxFails)
            {
                CheckHit();
            }
        }
    }

    private void CheckHit()
    {
        PlayHammerAnimation();
        if (IsAngleInZone(currentAngle, perfectAngleMin, perfectAngleMax))
        {
            PopUpAnimation(2);
            Debug.Log("PERFECT Strike!");
            score += 34;
            currentRound++;
            StartNewRound();
        }
        
        else if (IsAngleInZone(currentAngle, successAngleMin, successAngleMax))
        {
            PopUpAnimation(1);
            Debug.Log("Good Strike.");
            score += 20;
            currentRound++;
            StartNewRound();
        }

        else
        {
            PopUpAnimation(0);
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


    #region Animation

    private void PlayHammerAnimation()
    {
        anim.Play("Hammering", 0, 0f);
    }

    private void PopUpAnimation(int index)
    {
        if (index == 0)
        {
            GameObject popUp = Instantiate(missHitFX, popUpPos, false);
        }
        else if(index == 1)
        {
            GameObject popUp = Instantiate(goodHitFX, popUpPos, false);
        }
        else
        {
            GameObject popUp = Instantiate(perfectHitFX, popUpPos, false);
        }
        
    }

    private void HandleHotMetalAnimation()
    {
        float maxHeight = (currentRound * 45f) + 45f;

        if (hotMetal == null) { return; }
        if (hotness == null) { return; }

        hotMetal.sizeDelta = new Vector2(52f, maxHeight);
        hotness.alpha = Mathf.Floor((maxRounds - currentRound)/maxRounds);
    }

    #endregion

    private void UpdateProgressBarUI()
    {
        if (progressBar == null) { return; }
        progressBar.maxValue = maxRounds;
        progressBar.value = currentRound;
    }

    private void UpdateHeart()
    {
        if (heart1 != null && heart2 != null && heart3 != null)
        {
            if (currentFails <= 0)
            {
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;
            }
            else if (currentFails == 1)
            {
                heart1.sprite = heartBroken;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;
            }
            else if (currentFails == 2)
            {
                heart1.sprite = heartBroken;
                heart2.sprite = heartBroken;
                heart3.sprite = heartFull;
            }
            else if (currentFails >= 3)
            {
                heart1.sprite = heartBroken;
                heart2.sprite = heartBroken;
                heart3.sprite = heartBroken;
            }
        }
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
