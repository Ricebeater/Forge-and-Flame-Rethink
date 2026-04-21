using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SmelthingMinigame : MinigameBase
{
    [Header("Game Settings")]
    [SerializeField] private float timePerRound = 3f;
    [SerializeField] private int maxRounds = 3;
    [SerializeField] private int maxFails = 3;

    [Header("UI References")]
    [SerializeField] private Image[] arrowDisplays;
    [SerializeField] private Slider timerSlider;

    [Header("UI Sprites")]
    [SerializeField] private Sprite upArrowSprite;
    [SerializeField] private Sprite downArrowSprite;
    [SerializeField] private Sprite leftArrowSprite;
    [SerializeField] private Sprite rightArrowSprite;
    [SerializeField]private Image oreImage;
    [SerializeField]private Image gemImage;
    [SerializeField] private Color arrowColor;
    [SerializeField] private Color arrowColorGray;
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;
    [SerializeField] private Sprite heartBroken;
    [SerializeField] private Sprite heartFull;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    
    private List<KeyCode> currentSequence = new List<KeyCode>();
    private int currentInputIndex = 0;
    private int roundsWon = 0;
    private int currentRound = 0;
    private int fails = 0;
    private float timeRemaining;

    private float _score;
    

    public override void StartGame()
    {
        base.StartGame();

        SetCraftingMaterialUI();

        _score = 0;
        score = 0;
        currentRound = 0;
        roundsWon = 0;
        fails = 0;

        if (timerSlider != null)
        {
            timerSlider.maxValue = timePerRound;
        }

        StartNewRound();
    }

    private void Update()
    {
        if (!isPlaying) return;

        HandleTimer();
        HandleInput();
        UpdateHeart();
    }

    private void StartNewRound()
    {
        currentInputIndex = 0;
        timeRemaining = timePerRound;
        GenerateSequence();
        UpdateArrowUI();
    }

    private void GenerateSequence()
    {
        currentSequence.Clear();
        KeyCode[] possibleKeys = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

        for (int i = 0; i < 4; i++)
        {
            KeyCode randomKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
            currentSequence.Add(randomKey);
        }
    }

    private void UpdateArrowUI()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < arrowDisplays.Length)
            {
                arrowDisplays[i].color = arrowColorGray;

                switch (currentSequence[i])
                {
                    case KeyCode.UpArrow: arrowDisplays[i].sprite = upArrowSprite; break;
                    case KeyCode.DownArrow: arrowDisplays[i].sprite = downArrowSprite; break;
                    case KeyCode.LeftArrow: arrowDisplays[i].sprite = leftArrowSprite; break;
                    case KeyCode.RightArrow: arrowDisplays[i].sprite = rightArrowSprite; break;
                }
            }
        }
    }

    private void HandleTimer()
    {
        timeRemaining -= Time.deltaTime;

        if (timerSlider != null)
        {
            timerSlider.value = timeRemaining;
        }

        if (timeRemaining <= 0)
        {
            Debug.Log("Time ran out!");
            FailAttempt();
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            KeyCode expectedKey = currentSequence[currentInputIndex];

            if (Input.GetKeyDown(expectedKey))
            {
                arrowDisplays[currentInputIndex].color = arrowColor;
                currentInputIndex++;

                if (currentInputIndex >= currentSequence.Count)
                {
                    RoundWon();
                }
            }
            else
            {
                Debug.Log("Wrong arrow pressed!");
                FailAttempt();
            }
        }
    }

    private void RoundWon()
    {
        currentRound++;
        roundsWon++;

        PlayBellowAnimation();

        Debug.Log($"Round {roundsWon} Won!");

        if (currentRound >= maxRounds)
        {
            
            if(fails > 0)
            {
                _score = 99 - (fails * 33);
                score = Mathf.RoundToInt(_score / 20);
            }
            else
            {
                _score = 100;
                score = _score / 20;
            }
            Debug.Log("All 3 rounds successful! Perfect Score: " + score);
            EndGame();
        }
        else
        {
            _score += 33;
            StartNewRound();
        }
    }

    private void FailAttempt()
    {
        currentRound++;
        fails++;
        Debug.Log($"Failed! Chances used: {fails}/{maxFails}");

        if (currentRound >= maxFails)
        {
            Debug.Log($"Minigame Failed entirely. Final Score: {score}");
            EndGame();
        }
        else
        {
            StartNewRound();
        }
    }

    private void SetCraftingMaterialUI()
    {
        ItemData oreData = OrderManager.Instance.GetSelectedOreData();
        ItemData gemData = OrderManager.Instance.GetSelectedElementData();

        oreImage.sprite = oreData.icon;
        gemImage.sprite = gemData.icon;
    }

    private void UpdateHeart()
    {
        if (heart1 != null && heart2 != null && heart3 != null)
        {
            if (fails <= 0)
            {
                heart1.sprite = heartFull;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;
            }
            else if (fails == 1)
            {
                heart1.sprite = heartBroken;
                heart2.sprite = heartFull;
                heart3.sprite = heartFull;
            }
            else if (fails == 2)
            {
                heart1.sprite = heartBroken;
                heart2.sprite = heartBroken;
                heart3.sprite = heartFull;
            }
            else if (fails >= 3)
            {
                heart1.sprite = heartBroken;
                heart2.sprite = heartBroken;
                heart3.sprite = heartBroken;
            }
        }
    }

    public int GetSmeltScore()
    {
        return (int)score;
    }
    
    private void PlayBellowAnimation()
    {
        if(anim == null) { return; }
        anim.Play("Blow", 0, 0f);
    }
}
