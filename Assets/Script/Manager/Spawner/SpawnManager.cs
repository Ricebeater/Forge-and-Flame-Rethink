using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    [Header("Global Settings")]
    public Transform player;
    public TMP_Text timerText;

    [Header("Boss Settings")]
    public float timeUntilBoss = 60f;
    public GameObject finalBossPrefab;
    public float bossSpawnDelay = 5f;

    private bool isBossActive = false;

    private BaseSpawner[] allSpawnersInScene;

    void Start()
    {
        allSpawnersInScene = FindObjectsByType<BaseSpawner>(FindObjectsSortMode.None);

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Update()
    {
        if (!isBossActive)
        {
            timeUntilBoss -= Time.deltaTime;
            UpdateTimerUI();

            if (timeUntilBoss <= 0)
            {
                StartBossEvent();
            }
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(Mathf.Max(timeUntilBoss, 0) / 60F);
            int seconds = Mathf.FloorToInt(Mathf.Max(timeUntilBoss, 0) % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void StartBossEvent()
    {
        isBossActive = true;
        timeUntilBoss = 0;

        foreach (var spawner in allSpawnersInScene)
        {
            spawner.isActive = false;
            spawner.ClearEnemies();
        }

        StartCoroutine(BossSpawnSequence());
    }

    IEnumerator BossSpawnSequence()
    {
        float delayTimer = bossSpawnDelay;
        while (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"WARNING\nBOSS IN {Mathf.CeilToInt(delayTimer)}";
                timerText.color = Color.Lerp(Color.yellow, Color.red, Mathf.PingPong(Time.time * 5, 1));
            }
            yield return null;
        }

        if (timerText != null)
        {
            timerText.text = "BOSS FIGHT!";
            timerText.color = Color.red;
        }

        SpawnFinalBoss();
    }

    void SpawnFinalBoss()
    {
        if (finalBossPrefab == null || player == null) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y) * 15f;
        Vector3 spawnPos = player.position + spawnOffset;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 10.0f, NavMesh.AllAreas))
        {
            Instantiate(finalBossPrefab, hit.position, Quaternion.identity);
        }
    }
}