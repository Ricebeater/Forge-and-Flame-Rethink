using UnityEngine;

public class PlayAtState : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private CraftingStep step;
    [SerializeField] private bool isPlayed = false;

    private void Start()
    {
        isPlayed = false;
    }

    private void Update()
    {
        if (SmithingManager.Instance.IsCurrentStep(step) && !isPlayed)
        {
            isPlayed = true;
            anim.Play("Toss", 0, 0f);
        }

        if (!SmithingManager.Instance.IsCurrentStep(step))
        {
            isPlayed = false;
        }
    }
}
