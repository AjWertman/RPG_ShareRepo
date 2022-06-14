using RPGProject.Core;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class CharacterSelectMenu : MonoBehaviour
    {
        [SerializeField] Transform buttonContainer = null;
        [SerializeField] GameObject characterSelectButton = null;

        [SerializeField] Button backButton = null;

        public event Action<PlayableCharacter> onCharacterSelect;
        public event Action onBackToMainCoreMenu;

        Dictionary<Button, PlayableCharacter> characterButtons = new Dictionary<Button, PlayableCharacter>();

        private void Awake()
        {
            CreateCharacterButtons();
            backButton.onClick.AddListener(() => onBackToMainCoreMenu());

            foreach (Transform transform in buttonContainer)
            {
                transform.gameObject.SetActive(false);
            }
        }

        private void CreateCharacterButtons()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject buttonInstance = Instantiate(characterSelectButton, buttonContainer);
                Button button = buttonInstance.GetComponent<Button>();

                characterButtons.Add(button, null);

                button.onClick.AddListener(() => onCharacterSelect(characterButtons[button]));
                button.onClick.AddListener(() => ForceDeactivateHighlight(buttonInstance.GetComponent<MenuItemHighlighter>()));
            }
        }

        public void SetupCharacterSelectMenu(List<PlayableCharacter> _characters)
        {
            ResetCharacterSelectMenu();

            foreach (PlayableCharacter character in _characters)
            {
                Button button = GetAvailableCharacterButton();
                button.GetComponentInChildren<TextMeshProUGUI>().text = character.GetName();
                characterButtons[button] = character;
                button.gameObject.SetActive(true);
            }
        }

        private void ResetCharacterSelectMenu()
        {
            List<Button> buttonsToReset = new List<Button>();

            foreach (Button button in characterButtons.Keys)
            {
                buttonsToReset.Add(button);
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
                button.gameObject.SetActive(false);
            }

            foreach (Button button in buttonsToReset)
            {
                characterButtons[button] = null;
            }
        }

        private void ForceDeactivateHighlight(MenuItemHighlighter _highlighter)
        {
            _highlighter.ForceUnhighlight();
        }

        private Button GetAvailableCharacterButton()
        {
            Button availableButton = null;

            foreach (Button button in characterButtons.Keys)
            {
                if (characterButtons[button] == null)
                {
                    availableButton = button;
                    break;
                }
            }

            return availableButton;
        }
    }
}