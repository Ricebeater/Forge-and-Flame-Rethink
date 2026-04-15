using UnityEngine;

public class MinigameBase : MonoBehaviour
{
    [Header("Base Setting")]
    public CanvasGroup minigameCanvas;
    protected bool isPlaying = false;
    protected float score;

    public virtual void StartGame()
    {
        isPlaying = true;
        if (minigameCanvas != null)
        {
            minigameCanvas.alpha = 1f;
            minigameCanvas.interactable = true;
            minigameCanvas.blocksRaycasts = true;
        }
    }

    public virtual void EndGame()
    {
        isPlaying = false;
        if (minigameCanvas != null)
        {
            minigameCanvas.alpha = 0f;
            minigameCanvas.interactable = false;
            minigameCanvas.blocksRaycasts = false;
        }

        SmithingManager.Instance.CompleteCurrentStep(score);
    }

}
