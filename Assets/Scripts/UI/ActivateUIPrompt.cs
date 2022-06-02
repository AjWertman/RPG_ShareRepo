using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
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
