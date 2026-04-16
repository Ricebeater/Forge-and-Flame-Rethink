using UnityEngine;

public class MinigameBase : MonoBehaviour
{
    [Header("Base Setting")]
    public CanvasGroup minigameCanvas;
    protected bool isPlaying = false;
    protected float score;

    private void Start()
    {
        HideMinigameUI();
    }

    public virtual void StartGame()
    {
        isPlaying = true;
        ShowMinigameUI();
    }

    public virtual void EndGame()
    {
        isPlaying = false;
        HideMinigameUI();

        SmithingManager.Instance.CompleteCurrentStep(score);
    }

    public virtual void ShowMinigameUI()
    {
        if (minigameCanvas != null)
        {
            minigameCanvas.alpha = 1f;
            minigameCanvas.interactable = true;
            minigameCanvas.blocksRaycasts = true;
        }
    }

    public virtual void HideMinigameUI()
    {
        if (minigameCanvas != null)
        {
            minigameCanvas.alpha = 0f;
            minigameCanvas.interactable = false;
            minigameCanvas.blocksRaycasts = false;
        }
    }

}
