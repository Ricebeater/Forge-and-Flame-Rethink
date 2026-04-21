using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public void RestartEverything()
    {
        MoneyManager.Instance.playerMoney = 0;
        MoneyManager.Instance.currentDay = 0;
        InventoryManager.Instance.ResetData();

        SceneManager.LoadScene("MainMenu");
    }
}
