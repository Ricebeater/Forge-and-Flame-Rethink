using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [Header("Data")]
    public Transform player;
    public TMP_Text timerText;

    [Header("Audio Settings")]
    public AudioClip ambientMusic;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;
    public AudioClip bossMusic;
    [Range(0f, 1f)] public float bossVolume = 0.8f;

    [Header("Timer Settings")]
    public float timeUntilBoss = 60f;
    private bool isBossActive = false;

    [Header("Normal Enemy Settings")]
    public GameObject enemyPrefab;
    public int maxEnemy = 10;

    [Header("Special Enemy Settings")]
    public GameObject[] specialEnemyPrefabs;
    public int maxSpecialEnemy = 4;
    [Range(0, 100)] public float specialSpawnChance = 20f;

    [Header("Final Boss Settings")]
    public GameObject finalBossPrefab;
    public float bossSpawnDelay = 5f;

    [Header("Spawn Area Settings")]
    public float spawnRadius = 20f;
    public float minSpawnDistance = 5f;
    public float spawnInterval = 2f;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private List<GameObject> currentSpecialEnemies = new List<GameObject>();

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusic(ambientMusic, ambientVolume);
        }

        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (!isBossActive)
        {
            timeUntilBoss -= Time.deltaTime;

            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeUntilBoss / 60F);
                int seconds = Mathf.FloorToInt(timeUntilBoss % 60F);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                timerText.color = Color.white;
            }

            if (timeUntilBoss <= 0)
            {
                StartBossEvent();
            }
        }
    }

    void StartBossEvent()
    {
        isBossActive = true;
        timeUntilBoss = 0;

        ClearAllEnemies();

        StartCoroutine(BossSpawnSequence());
    }

    IEnumerator BossSpawnSequence()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMusicWithFade(bossMusic, bossVolume);
        }

        float delayTimer = bossSpawnDelay;

        while (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = $"WARNING\nBOSS IN {Mathf.CeilToInt(delayTimer)}";

                float blink = Mathf.PingPong(Time.time * 5, 1);
                timerText.color = Color.Lerp(Color.yellow, Color.red, blink);
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
        if (finalBossPrefab == null) return;

        Vector3 spawnPos = GetRandomPointAroundPlayer();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 5.0f, NavMesh.AllAreas))
        {
            Instantiate(finalBossPrefab, hit.position, Quaternion.identity);
            Debug.Log("Final Boss Spawned!");
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (!isBossActive)
        {
            currentEnemies.RemoveAll(item => item == null);
            currentSpecialEnemies.RemoveAll(item => item == null);

            bool spawnedSpecial = false;

            if (currentSpecialEnemies.Count < maxSpecialEnemy)
            {
                if (Random.Range(0f, 100f) <= specialSpawnChance && specialEnemyPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, specialEnemyPrefabs.Length);
                    SpawnEnemy(specialEnemyPrefabs[randomIndex], currentSpecialEnemies);
                    spawnedSpecial = true;
                }
            }

            if (!spawnedSpecial && currentEnemies.Count < maxEnemy)
            {
                SpawnEnemy(enemyPrefab, currentEnemies);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("Stopping Normal Spawns.");
    }

    void ClearAllEnemies()
    {
        foreach (var enemyObj in currentEnemies)
        {
            if (enemyObj != null)
            {
                EnemyAI ai = enemyObj.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.StartFleeing();
                }
                else
                {
                    Destroy(enemyObj);
                }
            }
        }

        foreach (var enemyObj in currentSpecialEnemies)
        {
            if (enemyObj != null)
            {
                EnemyAI ai = enemyObj.GetComponent<EnemyAI>();
                if (ai != null) ai.StartFleeing();
                else Destroy(enemyObj);
            }
        }

        currentEnemies.Clear();
        currentSpecialEnemies.Clear();
    }

    void SpawnEnemy(GameObject prefabToSpawn, List<GameObject> listTracking)
    {
        if (prefabToSpawn == null) return;
        Vector3 randomPoint = GetRandomPointAroundPlayer();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            GameObject newEnemy = Instantiate(prefabToSpawn, hit.position, Quaternion.identity);
            listTracking.Add(newEnemy);
        }
    }

    Vector3 GetRandomPointAroundPlayer()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, spawnRadius);
        Vector3 spawnOffset = new Vector3(randomCircle.x, 0, randomCircle.y) * randomDistance;
        return player.position + spawnOffset;
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}