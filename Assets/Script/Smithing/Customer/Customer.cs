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

    private bool isDialogPlaying = false;

    private void Start()
    {
        textBubble.SetActive(false);
        nameContainer.SetActive(false);
    }

    private void Update()
    {
        if(isDialogPlaying)
        {
            if(Input.GetKeyDown(KeyCode.E) && SmithingManager.Instance.IsCurrentStep(CraftingStep.Idle))
            {
                textBubble.SetActive(false);
                nameContainer.SetActive(false);
                OrderManager.Instance.ShowWeaponSelectMenu();
            }
        }

        if (SmithingManager.Instance.IsCurrentStep(CraftingStep.Delivering))
        {
            if (dialogText != null && customerSO != null)
            {
                dialogText.text = customerSO.goodFinish;
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                SmithingManager.Instance.CompleteCurrentStep(0);
            }
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
