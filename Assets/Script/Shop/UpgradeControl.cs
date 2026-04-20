using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeControl : MonoBehaviour
{
    [Header("Paper")]
    [SerializeField] private GameObject paper;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("End of Day")]
    [SerializeField] private TextMeshProUGUI currentDayText;
    [SerializeField] private TextMeshProUGUI customersTodayText;
    [SerializeField] private TextMeshProUGUI moneyEarnText;
    [SerializeField] private TextMeshProUGUI todayQuotaText;
    [SerializeField] private TextMeshProUGUI moneyRemainText;

    private float moneyEarnToday;
    private float moneyRemain;
    private float todayQuota;

    [Header("Upgrade")]
    [SerializeField] private Slider healthUpgradeSlider;
    [SerializeField] private Slider speedUpgradeSlider;
    [SerializeField] private Slider damageUpgradeSlider;
    [SerializeField] private Slider backpackUpgradeSlider;

    [SerializeField] private TextMeshProUGUI healthUpgradePriceText;
    [SerializeField] private TextMeshProUGUI speedUpgradePriceText;
    [SerializeField] private TextMeshProUGUI damageUpgradePriceText;
    [SerializeField] private TextMeshProUGUI backpackUpgradePriceText;
    
    private float healthUpgradePrice;
    private float speedUpgradePrice;
    private float damageUpgradePrice;
    private float backpackUpgradePrice;
    private enum UpgradeType { 
        health,
        speed,
        damage,
        backpack,
    };

    [Header("Animation")]
    [SerializeField] private GameObject notEnoughMoneyPopUp;
    [SerializeField] private Transform popUpPos;

    private void Start()
    {
        
    }

    private void Update()
    {
        ShowPaper();
        SummarizeDay();
        SetUpgradeSlider();
        SetUpgradePrice();
        
    }

    private void SummarizeDay()
    {
        float playerMoney = MoneyManager.Instance.playerMoney;

        //data
        moneyEarnToday = CustomerManager.Instance.totalMoneyEarnToday;
        todayQuota = GetTodayQuota();
        moneyRemain = playerMoney - todayQuota;

        //text
        currentDayText.text = MoneyManager.Instance.currentDay.ToString();
        customersTodayText.text = CustomerManager.Instance.customerSpawnedToday.ToString();
        moneyEarnText.text = moneyEarnToday.ToString();
        todayQuotaText.text = "-" + todayQuota.ToString();
        moneyRemainText.text = moneyRemain.ToString();
    }

    private void ShowPaper()
    {
        if(paper == null || canvasGroup == null) { return; }

        paper.SetActive(CustomerManager.Instance.isDayEnd);
        canvasGroup.interactable = CustomerManager.Instance.isDayEnd;
        canvasGroup.blocksRaycasts = CustomerManager.Instance.isDayEnd;
        
        if (CustomerManager.Instance.isDayEnd) { canvasGroup.alpha = 1.0f; }
        else { canvasGroup.alpha = 0.0f; }
    }

   
    private float GetTodayQuota()
    {
        float dayNum = MoneyManager.Instance.currentDay;
        return (dayNum * 10) + ((dayNum - 1) * 5);
    }

    #region Upgrades
    
    private void SetUpgradeSlider()
    {
        healthUpgradeSlider.value   = InventoryManager.Instance.healthLv;
        speedUpgradeSlider.value    = InventoryManager.Instance.speedLv;
        damageUpgradeSlider.value   = InventoryManager.Instance.damageLv;
        backpackUpgradeSlider.value = InventoryManager.Instance.backpackLv;
    }

    private void SetUpgradePrice()
    {
        healthUpgradePrice      = (InventoryManager.Instance.healthLv * 5) + 10;
        speedUpgradePrice       = (InventoryManager.Instance.speedLv * 5) + 10;
        damageUpgradePrice      = (InventoryManager.Instance.damageLv * 5) + 10;
        backpackUpgradePrice    = (InventoryManager.Instance.backpackLv * 5) + 10;

        if (InventoryManager.Instance.healthLv >= 3)    { healthUpgradePriceText.text = "Max"; }
        else { healthUpgradePriceText.text = healthUpgradePrice.ToString() + "c"; }

        if (InventoryManager.Instance.speedLv >= 3)     { speedUpgradePriceText.text = "Max"; }
        else { speedUpgradePriceText.text = speedUpgradePrice.ToString() + "c"; }

        if (InventoryManager.Instance.damageLv >= 3)    { damageUpgradePriceText.text = "Max"; }
        else { damageUpgradePriceText.text = damageUpgradePrice.ToString() + "c"; }

        if (InventoryManager.Instance.backpackLv >= 3) { backpackUpgradePriceText.text = "Max"; }
        else { backpackUpgradePriceText.text = backpackUpgradePrice.ToString() + "c"; }


    }

    #endregion


    #region Button

    public void TryUpgrade(int upgradeIndex)
    {
        UpgradeType selectedUpgrade = (UpgradeType)upgradeIndex;
        float playerMoney = MoneyManager.Instance.playerMoney;

        if(selectedUpgrade == UpgradeType.health)
        { 
            if(playerMoney - todayQuota >= healthUpgradePrice && InventoryManager.Instance.healthLv < 3)
            {
                InventoryManager.Instance.healthLv += 1;
                MoneyManager.Instance.playerMoney -= (int)healthUpgradePrice;
            }
            
            else if(playerMoney - todayQuota < healthUpgradePrice)
            {
                NotEnoughMoneyPopUp();
            }
        }

        if(selectedUpgrade == UpgradeType.speed)
        {
            if (playerMoney - todayQuota >= speedUpgradePrice && InventoryManager.Instance.speedLv < 3)
            {
                InventoryManager.Instance.speedLv += 1;
                MoneyManager.Instance.playerMoney -= (int)speedUpgradePrice;
            }

            else if (playerMoney - todayQuota < speedUpgradePrice)
            {
                NotEnoughMoneyPopUp();
            }
        }

        if (selectedUpgrade == UpgradeType.damage)
        {
            if (playerMoney - todayQuota     >= damageUpgradePrice && InventoryManager.Instance.damageLv < 3)
            {
                InventoryManager.Instance.damageLv += 1;
                MoneyManager.Instance.playerMoney -= (int)damageUpgradePrice;
            }
            
            else if (playerMoney - todayQuota < damageUpgradePrice)
            {
                NotEnoughMoneyPopUp();
            }
        }

        if (selectedUpgrade == UpgradeType.backpack)
        {
            if (playerMoney - todayQuota >= backpackUpgradePrice && InventoryManager.Instance.backpackLv < 3)
            {
                InventoryManager.Instance.backpackLv += 1;
                MoneyManager.Instance.playerMoney -= (int)backpackUpgradePrice;
            }

            else if (playerMoney - todayQuota < backpackUpgradePrice)
            {
                NotEnoughMoneyPopUp();
            }
        }
    }

    public void NotEnoughMoneyPopUp()
    {
        if (popUpPos != null || notEnoughMoneyPopUp != null)
        {
            GameObject popUp = Instantiate(notEnoughMoneyPopUp, popUpPos, false);
        }
    }

    public void PayQuota()
    {
        CustomerManager.Instance.isDayEnd = false;
        MoneyManager.Instance.playerMoney -= ((int)todayQuota);
        SceneManager.LoadScene("Huntgame");

    }

    #endregion
}

