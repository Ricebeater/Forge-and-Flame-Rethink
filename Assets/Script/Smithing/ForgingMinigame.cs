using UnityEngine;

public class ForgingMinigame : MinigameBase
{
    private void Start()
    {
        score = 0;
    }

    public void AddScore()
    {
        score++;
        Debug.Log("Forge Score : " + score);
    }

    public int GetForgeScore()
    {
        return (int)score;
    }
}
