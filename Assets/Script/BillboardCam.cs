using UnityEngine;

public class BillboardCam : MonoBehaviour
{
    public Camera mainCamera;
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        transform.forward = mainCamera.transform.forward;
    }
}