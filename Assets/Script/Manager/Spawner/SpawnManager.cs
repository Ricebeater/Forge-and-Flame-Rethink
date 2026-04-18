using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Global Settings")]
    public Transform player;
    public TMP_Text timerText;

    [Header("Resource Phase Settings")]
    public float gatheringTime = 60f;
    private float timeRemaining;
    private bool isPhaseActive = false;

    [Header("Audio Settings")]
    public AudioClip ambientMusic;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;

    private BaseSpawner[] allSpawnersInScene;

    void Start()
    {
        allSpawnersInScene = FindObjectsByType<BaseSpawner>(FindObjectsSortMode.None);
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (SoundManager.Instance != null && ambientMusic != null)
            SoundManager.Instance.PlayMusic(ambientMusic, ambientVolume);

        timeRemaining = gatheringTime;
        StartGatheringPhase();
    }

    void Update()
    {
        if (isPhaseActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else EndGatheringPhase();
        }
    }

    void StartGatheringPhase()
    {
        isPhaseActive = true;
        foreach (var spawner in allSpawnersInScene) spawner.BeginSpawning();
    }

    void EndGatheringPhase()
    {
        isPhaseActive = false;
        timeRemaining = 0;
        UpdateTimerUI();
        foreach (var spawner in allSpawnersInScene) spawner.isActive = false;
        if (timerText != null) { timerText.text = "TIME'S UP!"; timerText.color = Color.red; }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"TIME LEFT: {Mathf.CeilToInt(timeRemaining)}s";
            if (timeRemaining <= 10f) timerText.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time * 5, 1));
            else timerText.color = Color.white;
        }
    }
}