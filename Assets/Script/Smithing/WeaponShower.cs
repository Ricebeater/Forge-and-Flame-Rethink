using UnityEngine;
using UnityEngine.UI;

public class WeaponShower : MonoBehaviour
{
    [SerializeField] private Image glow1;
    [SerializeField] private Image glow2;
    [SerializeField] private Image glow3;
    [SerializeField] private Image weapon;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        SetWeaponUI();
        HideWeaponShowerUI();
    }

    public void ShowWeaponShowerUI()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        SetWeaponUI();
    }

    public void HideWeaponShowerUI()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void SetWeaponUI() 
    {
        Color elementColor = OrderManager.Instance.GetSelectedElementColor();

        if (weapon != null)
        {
            weapon.sprite = OrderManager.Instance.GetSelectedWeaponSprite();
        }

        if (glow1 != null && glow2 != null && glow3 != null)
        {
            glow1.color = elementColor;
            glow2.color = elementColor;
            glow3.color = elementColor;
        }
    }

    public void CompleteStep()
    {
        HideWeaponShowerUI();
        SmithingManager.Instance.CompleteCurrentStep(0);
    }
}
