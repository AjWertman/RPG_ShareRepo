using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] Button nextButton = null;

    Animator animator = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButton);
    }

    private void OnNextButton()
    {
        animator.SetTrigger("next");
        animator.ResetTrigger("next");
    }
}
