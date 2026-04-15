using UnityEngine;

public class QuenchingMinigame : MinigameBase
{
    private void Start()
    {
        score = 0;
    }

    public void AddScore()
    {
        score++;
        Debug.Log("Quenching Score :" + score);
    }

    public int GetQuenchScore()
    {
        return (int)score;
    }
}
