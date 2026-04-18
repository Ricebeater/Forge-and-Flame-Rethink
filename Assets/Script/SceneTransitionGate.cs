using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionGate : MonoBehaviour
{
    [SerializeField] private string nextScene;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            GoToHuntGame();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GoToSmithGame();
        }
    }

    public void GoToSmithGame()
    {
        SaveCurrentInventory();
        SceneManager.LoadScene("SmithGame");
    }

    public void GoToHuntGame()
    {
        SceneManager.LoadScene("HuntGame");
    }

    private void SaveCurrentInventory()
    {
        PlayerInventory inventory = FindAnyObjectByType<PlayerInventory>();

        if (inventory != null)
        {
            //InventoryManager.Instance.SaveGridInventory(inventory);
            Debug.Log("Auto-saved inventory before switching scenes.");
        }
        else
        {
            Debug.LogWarning("Could not find a PlayerInventory to save!");
        }
    }

}
