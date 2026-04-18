using TMPro;
using UnityEngine;

public class PlayerEconomic : MonoBehaviour
{
    [Header("Money UI")]
    [SerializeField] private TextMeshProUGUI playerMoneyText;

    private void Update()
    {
        UpdateMoneyDisplay(MoneyManager.Instance.playerMoney);
    }

    private void UpdateMoneyDisplay(int money)
    {
        playerMoneyText.text = money.ToString();
    }
}
