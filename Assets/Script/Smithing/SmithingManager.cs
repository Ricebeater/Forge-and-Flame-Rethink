using UnityEngine;
using UnityEngine.Events;

public class SmithingManager : MonoBehaviour
{
    public static SmithingManager Instance { get; private set; }
    
    public CraftingStep currentCraftingStep { get; private set; } = CraftingStep.Idle;
    
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

    public void StartCrafting()
    {
        if (currentCraftingStep == CraftingStep.Idle)
        {
            currentCraftingStep = CraftingStep.Smelting;
            Debug.Log("Crafting started: Taking order.");
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
                forgingGame.StartGame();
                break;
            
            case CraftingStep.Forging:
                forgingScore = stepScore;
                currentCraftingStep = CraftingStep.Quenching;
                quenchingGame.StartGame();
                break;

            case CraftingStep.Quenching:
                quenchingScore = stepScore;
                currentCraftingStep = CraftingStep.Idle;
                FinalSmithingScore();
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
        int smeltScore = smelthingGame.GetSmeltScore();
        int forgeScore = forgingGame.GetForgeScore();
        int quenchScore = quenchingGame.GetQuenchScore();

        totalScore = smeltScore + forgeScore + quenchScore;
            Debug.Log($"Final Smithing Score: {totalScore}");
    }
}

public enum CraftingStep
{
    Idle,
    Smelting,
    Forging,
    Quenching
}
