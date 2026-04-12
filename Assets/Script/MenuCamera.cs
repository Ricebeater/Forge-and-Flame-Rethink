using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    [Header("Settings")]
    public Transform target;
    public float rotationSpeed = 5f;

    [Header("Offset (Optional)")]
    public float heightOffset = 2f;

    void LateUpdate()
    {
        if (target == null) return;

        transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

        Vector3 lookAtPos = target.position + Vector3.up * heightOffset;
        transform.LookAt(lookAtPos);
    }
}
