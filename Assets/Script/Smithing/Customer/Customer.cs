using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Customer : MonoBehaviour
{
    [Header("Customer Chat")]
    [SerializeField] private GameObject textBubble;
    [SerializeField] private GameObject nameContainer;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI nameText;

    [Header("Customer Data")]
    [SerializeField] private CustomerOrderSO customerSO;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    private bool isDialogPlaying = false;
    private Transform targetWaypoint;
    private Transform targetExitPoint;
    private bool isMoving = false;

    private bool hasOrdered = false;

    private void Start()
    {
        targetExitPoint = CustomerManager.Instance.GetExitPoint();
        textBubble.SetActive(false);
        nameContainer.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        if (isMoving && targetWaypoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
            {
                isMoving = false;
            }

        }

        if (SmithingManager.Instance.IsCurrentStep(CraftingStep.Delivering))
        {
            if(Vector3.Distance(transform.position, targetExitPoint.position) < 0.1f)
            {
                isMoving = false;
                CustomerManager.Instance.OnCustomerLeft();
                SmithingManager.Instance.CompleteCurrentStep(0);
                Destroy(gameObject);
            }
        }
    }

    private void HandleInteraction()
    {
        
        if (!isMoving && SmithingManager.Instance.IsCurrentStep(CraftingStep.Waiting))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasOrdered)
                {
                    SayHello();
                }   
                if (hasOrdered)
                {
                    OrderManager.Instance.currentCustomerOrder = customerSO;
                    OrderManager.Instance.ShowWeaponSelectMenu();
                }
                hasOrdered = true;
            }
        }

        if (!SmithingManager.Instance.IsCurrentStep(CraftingStep.Waiting))
        {
            textBubble.SetActive(false);
            nameContainer.SetActive(false);
        }
        
        if (SmithingManager.Instance.IsCurrentStep(CraftingStep.Delivering))
        {
            if (dialogText != null) 
            {
                if (SmithingManager.Instance.GetTotalScore() > 9) { dialogText.text = customerSO.goodFinish; }
                else if (SmithingManager.Instance.GetTotalScore() > 5) { dialogText.text = customerSO.okayFinish; }
                else if (SmithingManager.Instance.GetTotalScore() <= 4) { dialogText.text = customerSO.badFinish; }
                
            }
            textBubble.SetActive(true);
            nameContainer.SetActive(true);
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                LeaveShop();
            }
        }        
    }

    public void GoToCounter(Transform counterTransform)
    {
        targetWaypoint = counterTransform;
        transform.LookAt(new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z));
        isMoving = true;
    }

    private void LeaveShop()
    {
        textBubble.SetActive(false);
        nameContainer.SetActive(false);

        if (CustomerManager.Instance != null)
        {
            targetWaypoint = CustomerManager.Instance.GetExitPoint();
            transform.LookAt(new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z));
            isMoving = true;
        }
    }

    public void SayHello()
    {
        Debug.Log("Hello, It's you, The purest you");
        PlayDialog();
    }


    private void PlayDialog()
    {
        if(textBubble == null) { return; }
        if (nameText == null) { return; }
        
        dialogText.text = customerSO.orderDescription;
        nameText.text = customerSO.customerName + " :";
        textBubble.SetActive(true);
        nameContainer.SetActive(true);

        isDialogPlaying = true;
    }
}
