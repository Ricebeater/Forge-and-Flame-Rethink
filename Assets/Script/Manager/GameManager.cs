using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool isWin = false;

    [Header("Game References")]
    public PlayerController player;
    public SpawnManager spawner;

    [Header("Cameras")]
    public Camera menuCamera;
    public Camera playerCamera;

    [Header("UI")]
    public GameObject menuCanvas;
    public Canvas[] inGameUICanvases;
    public CanvasGroup endScreenCanvasGroup;
    public TextMeshProUGUI endGameText;

    [SerializeField] private GameObject transitionEffect;
    [SerializeField] private Transform transitionEffectPos;

    [Header("Audio")]
    public AudioClip menuMusic;
    [Range(0f, 1f)] public float menuVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ShowMainMenu();
        Time.timeScale = 1f;
    }

    void ShowMainMenu()
    {
        if (menuCamera != null) menuCamera.enabled = true;
        if (playerCamera != null) playerCamera.enabled = false;

        if (menuCanvas != null) menuCanvas.SetActive(true);
        ToggleInGameUI(false);
        if (endScreenCanvasGroup != null) endScreenCanvasGroup.alpha = 1;

        if (spawner != null) spawner.enabled = false;
        
        if (player != null)
        {
            player.enabled = true;
            player.SetControl(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (SoundManager.Instance != null && menuMusic != null)
        {
            SoundManager.Instance.PlayMusic(menuMusic, menuVolume);
        }
    }

    public void StartGame()
    {
        if (menuCanvas != null) menuCanvas.SetActive(false);
        ToggleInGameUI(true);

        if (menuCamera != null) menuCamera.enabled = false;
        if (playerCamera != null) playerCamera.enabled = true;

        if (spawner != null) spawner.enabled = true;
        
        if (player != null)
        {
            player.enabled = true;
            player.SetControl(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void TriggerEndGame()
    {
        Debug.Log("BOSS DEFEATED! YOU WIN!");
        EndWithTransition();
        //StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        ToggleInGameUI(false);

        Time.timeScale = 0.2f;
       
        if (spawner != null)
        {
            if (spawner.timerText != null) spawner.timerText.text = "";
            spawner.enabled = false;
        }

        if (player != null) player.SetControl(false);

        if (SoundManager.Instance != null) SoundManager.Instance.StopMusic();

        yield return new WaitForSecondsRealtime(2f);

        if (endScreenCanvasGroup != null)
        {
            endScreenCanvasGroup.blocksRaycasts = true;
            float timer = 0f;
            float duration = 3f;

            if(endGameText != null)
            {
                if (isWin)
                {
                    endGameText.text = $"you win...\nbut at what cost";
                }
                else
                {
                    endGameText.text = $"What a shame...\nstupid human";
                }
            }

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                endScreenCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
                yield return null;
            }
            endScreenCanvasGroup.alpha = 1f;
        }

        yield return new WaitForSecondsRealtime(4f);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void EndWithTransition()
    {
        if (transitionEffect != null)
        {
            GameObject transition = Instantiate(transitionEffect, transitionEffectPos, false);
        }
    }

    void ToggleInGameUI(bool isActive)
    {
        if (inGameUICanvases != null)
        {
            foreach (var canvas in inGameUICanvases)
            {
                if (canvas != null) canvas.enabled = isActive;
            }
        }
    }
}