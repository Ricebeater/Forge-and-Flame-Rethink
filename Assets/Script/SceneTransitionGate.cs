using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionGate : MonoBehaviour
{
    [SerializeField] private string nextScene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = FindAnyObjectByType<PlayerInventory>();
            InventoryManager.Instance.SavePlayerInventory(inventory);

            SceneManager.LoadScene(nextScene);
        }
    }
}
