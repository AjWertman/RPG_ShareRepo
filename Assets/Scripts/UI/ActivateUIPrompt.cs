using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
    /// <summary>
    /// Piece of text near the cursor to indicate what the player 
    /// would activate on click.
    /// </summary>
    public class ActivateUIPrompt : MonoBehaviour
    {
        [SerializeField] GameObject activationPromptObject = null;
        TextMeshProUGUI text = null;

        private void Awake()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void ActivateActivateUI(string _whatToActivate)
        {
            text.text = _whatToActivate;
            activationPromptObject.SetActive(true);
        }

        public void DeactivateActivateUI()
        {
            text.text = "";
            activationPromptObject.SetActive(false);
        }

        public GameObject GetActivationPromptObject()
        {
            return activationPromptObject;
        }
    }
}
