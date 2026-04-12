using UnityEngine;

public class Identity : MonoBehaviour
{
    [Header("Identity Info")]
    public string entityName;

    private void OnValidate()
    {
        SyncName();
    }

    private void Awake()
    {
        SyncName();
    }

    public void SyncName()
    {
        if (!string.IsNullOrEmpty(entityName))
        {
            gameObject.name = entityName;
        }
    }

    public void SetName(string newName)
    {
        entityName = newName;
        SyncName();
    }
}