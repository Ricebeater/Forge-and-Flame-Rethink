using UnityEngine;
using UnityEngine.Events;

public class SmithingManager : MonoBehaviour
{
    public static SmithingManager Instance { get; private set; }
    
    public CraftingStep currentCraftingStep { get; private set; } = CraftingStep.Waiting;

    [Header("Minigames")]
    public SmelthingMinigame smelthingGame;
    public ForgingMinigame forgingGame;
    public QuenchingMinigame quenchingGame;

    private float smeltingScore;
    private float forgingScore;
    private float quenchingScore;
    private float totalScore;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        
    }

    public void CompleteCurrentStep(float stepScore)
    {
        totalScore += stepScore;

        switch (currentCraftingStep)
        {
            case CraftingStep.Smelting:
                smeltingScore = stepScore;
                currentCraftingStep = CraftingStep.Forging;
                Debug.Log("Current Step: Forging");
                forgingGame.StartGame();
                break;
            
            case CraftingStep.Forging:
                forgingScore = stepScore;
                currentCraftingStep = CraftingStep.Quenching;
                Debug.Log("Current Step: Quenching");
                quenchingGame.StartGame();
                break;

            case CraftingStep.Quenching:
                quenchingScore = stepScore;
                currentCraftingStep = CraftingStep.Delivering;
                Debug.Log("Current Step: Delivering");
                FinalSmithingScore();
                break;

            case CraftingStep.Delivering:
                currentCraftingStep = CraftingStep.Waiting;
                Debug.Log("Current Step: Waiting");
                break;
        }
    }

    public void StartSmithing()
    {
        currentCraftingStep = CraftingStep.Smelting;
        if (smelthingGame != null)
        {
            smelthingGame.StartGame();
        }
    }

    public void FinalSmithingScore()
    {
        totalScore = smeltingScore + forgingScore + quenchingScore;
        Debug.Log($"Final Smithing Score: {totalScore}");

        MoneyManager.Instance.AddMoney((int)totalScore);
    }

    public bool IsCurrentStep(CraftingStep step)
    {
        return currentCraftingStep == step;
    }

    public float GetTotalScore()
    {
        return totalScore;
    }

}

public enum CraftingStep
{
    Waiting,
    Smelting,
    Forging,
    Quenching,
    Delivering
}
