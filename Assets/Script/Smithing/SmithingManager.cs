using UnityEngine;

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
        if (currentCraftingStep == CraftingStep.Smelting)
        {
            currentCraftingStep = CraftingStep.TakeOrder;
            Debug.Log("Crafting started: Taking order.");
        }
    }

    private void Update()
    {
        
    }

    public void CompleteCurrentStep(float stepScore)
    {
        totalScore += stepScore;

        if(currentCraftingStep == CraftingStep.Smelting)
        {
            currentCraftingStep = CraftingStep.Smelting;
        }
        else if(currentCraftingStep == CraftingStep.Forging)
        {
            currentCraftingStep = CraftingStep.Forging;
        }
        else if(currentCraftingStep == CraftingStep.Quenching)
        {
            currentCraftingStep = CraftingStep.Quenching;
        }
        
    }

    public void StartSmithing()
    {
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
    TakeOrder,
    Smelting,
    Forging,
    Quenching,
    Deliver
}
