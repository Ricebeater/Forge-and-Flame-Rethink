using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public int playerMoney = 0;
    public int currentDay = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            
        }
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        Debug.Log($"Added {amount} money. Current money: {playerMoney}");
    }
}
