using UnityEngine;

public class IventoryManager : MonoBehaviour
{
    public static IventoryManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance = null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
