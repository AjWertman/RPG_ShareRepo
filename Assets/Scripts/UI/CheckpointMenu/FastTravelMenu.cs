using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// The menu that the player can use to fast travel.
    /// </summary>
    public class FastTravelMenu : MonoBehaviour
    {
        [SerializeField] GameObject fastTravelButtonPrefab = null;
        [SerializeField] RectTransform contentRectTransform = null;
        [SerializeField] int amountOfFastTravelButtons = 8;

        [SerializeField] Button backButton = null;

        Dictionary<Button, string> fastTravelButtons = new Dictionary<Button, string>();

        public event Action<string> onFastTravelButtonClick;
        public event Action onBackButton;

        public void InitalizeFastTravelMenu()
        {
            CreateFastTravelButtons();
            backButton.onClick.AddListener(()=> onBackButton());
        }

        private void CreateFastTravelButtons()
        {
            for (int i = 0; i < amountOfFastTravelButtons; i++)
            {
                GameObject buttonInstance = Instantiate(fastTravelButtonPrefab, contentRectTransform);
                Button button = buttonInstance.GetComponent<Button>();
                button.onClick.AddListener(() => onFastTravelButtonClick(fastTravelButtons[button]));

                button.GetComponentInChildren<TextMeshProUGUI>().text = "";

                fastTravelButtons.Add(button, null);

                buttonInstance.gameObject.SetActive(false);
            }
        }

        public void SetupFastTravelMenu(IEnumerable<string> _fastTravelIDs)
        {
            ResetFastTravelMenu();

            foreach (string fastTravelID in _fastTravelIDs)
            {
                Button fastTravelButton = GetAvailableButton();
                fastTravelButton.GetComponentInChildren<TextMeshProUGUI>().text = fastTravelID;

                fastTravelButtons[fastTravelButton] = fastTravelID;

                fastTravelButton.gameObject.SetActive(true);
            }
        }

        public void ResetFastTravelMenu()
        {
            List<Button> buttonsToReset = new List<Button>();

            foreach (Button fastTravelButton in fastTravelButtons.Keys)
            {
                buttonsToReset.Add(fastTravelButton);
            }

            foreach(Button fastTravelButton in buttonsToReset)
            {
                fastTravelButtons[fastTravelButton] = null;
                fastTravelButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
                fastTravelButton.gameObject.SetActive(false);
            }
        }

        private Button GetAvailableButton()
        {
            Button availableButton = null;

            foreach(Button fastTravelButton in fastTravelButtons.Keys)
            {
                if(fastTravelButtons[fastTravelButton] == null)
                {
                    availableButton = fastTravelButton;
                    break;
                }
            }

            return availableButton;
        }
    }
}