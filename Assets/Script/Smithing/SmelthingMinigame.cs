using UnityEngine;

public class SmelthingMinigame : MinigameBase
{
    private void Start()
    {
        score = 0;
    }

    public void AddScore()
    {
        score++;
        Debug.Log("Smelthing Score :" + score);
    }

    public int GetSmeltScore()
    {
        return (int)score;
    }
}
