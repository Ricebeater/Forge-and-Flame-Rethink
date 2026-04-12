using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Data")]
    public Slider hpSlider;
    public Character player;
    [Header("HUD Settings")]
    public float smoothSpeed = 5f;
    void Start()
    {
        if (player != null)
        {
            hpSlider.maxValue = player.maxHP;
            hpSlider.value = player.currentHP;
        }
    }

    void Update()
    {
        if (player != null)
        {
            hpSlider.value = Mathf.Lerp(hpSlider.value, player.currentHP, smoothSpeed * Time.deltaTime);
        }
    }
}