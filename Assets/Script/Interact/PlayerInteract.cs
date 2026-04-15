using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private CanvasGroup weaponSelectCanvas;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
            OrderManager.Instance.ShowWeaponSelectMenu();
        }
    }

    private void Interact()
    {
        Vector3 hitPoint = transform.position + (transform.forward * interactRange);

        Collider[] hitColliders = Physics.OverlapSphere(hitPoint, interactRadius);
        foreach (Collider hitColider in hitColliders)
        {
            Customer customer = hitColider.GetComponent<Customer>();
            if (customer != null)
            {
                Debug.Log("Interacted with customer: " + customer.name);
                customer.SayHello();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 hitPoint = transform.position + (transform.forward * interactRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(hitPoint, interactRadius);
    }
}
