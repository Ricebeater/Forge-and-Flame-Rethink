using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }
    

    [Header("Daily Setting")]
    [SerializeField] private int maxCustomersPerDay = 3;
    private int customerSpawnedToday = 0;

    [Header("Customer Spawner")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform targetPos;
    [SerializeField] private Transform exitPos;  
    [SerializeField] private GameObject[] customerList;

    private GameObject currentCustomer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartNewDay();
    }

    public void StartNewDay()
    {
        customerSpawnedToday = 0;
        SpawnNextCustomer();
        Debug.Log("A new day has started! Customers are ready to arrive.");
    }

    public void SpawnNextCustomer()
    {
        if(customerSpawnedToday >= maxCustomersPerDay)
        {
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

    public void OnCustomerLeft()
    {
        Invoke("SpawnNextCustomer", 2f);
    }

    public Transform GetExitPoint()
    {
        return exitPos;
    }
}
