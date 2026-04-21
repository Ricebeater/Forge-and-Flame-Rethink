using UnityEngine;

public class MenuCameraPan : MonoBehaviour
{
    public float swaySpeed = 0.2f;
    public float swayAngle = 5f;

    public float tiltSpeed = 0.15f;
    public float tiltAngle = 1.5f;

    private Quaternion startRot;

    void Start()
    {
        startRot = transform.rotation;
    }

    void Update()
    {
        float sinValueSway = Mathf.Sin(Time.time * swaySpeed);
        float sinValueTilt = Mathf.Sin(Time.time * tiltSpeed);

        Quaternion targetRotation = startRot * Quaternion.Euler(sinValueTilt * tiltAngle, sinValueSway * swayAngle, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
    }
}