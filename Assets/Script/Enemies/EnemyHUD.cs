using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    public Character enemyStats;

    [Header("Settings")]
    public bool hideWhenFull = true;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (enemyStats == null)
            enemyStats = GetComponentInParent<Character>();

        if (slider == null)
            slider = GetComponent<Slider>();

        if (enemyStats != null)
        {
            slider.maxValue = enemyStats.maxHP;
            slider.value = enemyStats.currentHP;
        }
    }

    void LateUpdate()
    {
        if (enemyStats == null) return;

        if (slider.maxValue != enemyStats.maxHP)
        {
            slider.maxValue = enemyStats.maxHP;
        }

        slider.value = enemyStats.currentHP;

        if (hideWhenFull)
        {
            if (slider.value >= slider.maxValue)
                slider.gameObject.SetActive(false);
            else
                slider.gameObject.SetActive(true);
        }

        if (mainCam != null)
        {
            transform.LookAt(transform.position + mainCam.transform.forward);
        }
    }
}