using System.Collections;
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
    public WeaponShower weaponShower;

    [Header("Effect")]
    [SerializeField] private GameObject finishBanner;

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

    private void Start()
    {
        finishBanner.SetActive(false);
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
                currentCraftingStep = CraftingStep.ShowWeapon;
                weaponShower.ShowWeaponShowerUI();
                Debug.Log("Current Step: Delivering");
                break;

            case CraftingStep.ShowWeapon:
                currentCraftingStep = CraftingStep.Delivering;
                FinalSmithingScore();
                Debug.Log("Current Step: Delivering");
                break;

            case CraftingStep.Delivering:
                currentCraftingStep = CraftingStep.Waiting;
                Debug.Log("Current Step: Waiting");
                break;
        }
    }

    public void StartSmithing()
    {
        totalScore = 0;

        currentCraftingStep = CraftingStep.Smelting;
        if (smelthingGame != null)
        {
            smelthingGame.StartGame();
        }
    }

    public float FinalSmithingScore()
    {
        Debug.Log($"Final Smithing Score: {totalScore}");
        MoneyManager.Instance.AddMoney((int)totalScore);
        CustomerManager.Instance.totalMoneyEarnToday += totalScore;

        return totalScore = smeltingScore + forgingScore + quenchingScore;
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
    ShowWeapon,
    Delivering
}
