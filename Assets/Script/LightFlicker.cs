using UnityEngine;

public class lightFlicker : MonoBehaviour
{
    public Light forgeLight;
    public float minIntensity;
    public float maxIntensity;
    void Update()
    {
        forgeLight.intensity = Mathf.Lerp(forgeLight.intensity, Random.Range(minIntensity, maxIntensity), Time.deltaTime * 10);
    }
}
