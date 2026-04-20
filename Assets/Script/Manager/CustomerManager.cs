using System.Collections;
using Unity.Hierarchy;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }
    
    [Header("Daily Setting")]
    [SerializeField] private int maxCustomersPerDay = 3;
    public int customerSpawnedToday = 0;
    public bool isDayEnd = false;
    public float totalMoneyEarnToday = 0;

    [Header("Customer Spawner")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform targetPos;
    [SerializeField] private Transform exitPos;  
    [SerializeField] private GameObject[] customerList;

    [SerializeField] private GameObject transitionIn;
    [SerializeField] private GameObject transitionOut;
    [SerializeField] private Transform transitionPos;

    
    private GameObject currentCustomer;

    private void Awake()
    {
        if (Instance == null)
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
        StartCoroutine(TransitionIn());
        StartNewDay();
    }

    public void StartNewDay()
    {
        isDayEnd = false;
        MoneyManager.Instance.currentDay++;
        totalMoneyEarnToday = 0;
        customerSpawnedToday = 0;
        SpawnNextCustomer();
        Debug.Log("A new day has started! Customers are ready to arrive.");
    }

    public void SpawnNextCustomer()
    {
        if(customerSpawnedToday >= maxCustomersPerDay )
        {
            EndDay();
            Debug.Log("Maximum customers for today reached!");
            return;
        }

        if (customerList.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, customerList.Length);
        GameObject selectedPrefab = customerList[randomIndex];

        currentCustomer = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);

        Customer customerScript = currentCustomer.GetComponent<Customer>();
        if (customerScript != null)
        {
            customerScript.GoToCounter(targetPos);
        }

        customerSpawnedToday++;
    }

    public float GetTotalMoneyEarnToday()
    {
        return totalMoneyEarnToday;
    }

    private void EndDay()
    {
        isDayEnd = true;
    }

    public void OnCustomerLeft()
    {
        Invoke("SpawnNextCustomer", 2f);
    }

    public Transform GetExitPoint()
    {
        return exitPos;
    }

    private IEnumerator TransitionIn()
    {
        if(transitionIn != null)
        {
            GameObject transIn = Instantiate(transitionIn, transitionPos, false);
        }

        yield return new WaitForSecondsRealtime(2f);
    }
}
