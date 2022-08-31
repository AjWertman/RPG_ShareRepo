using RPGProject.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public enum MainMenuButtonType { Character, Inventory, Quests, Map, Options, MainMenu }

    /// <summary>
    /// The core navigator of the player menu. 
    /// </summary>
    public class CoreMainMenu : MonoBehaviour
    {
        [SerializeField] Button characterButton = null;
        [SerializeField] Button inventoryButton = null;
        [SerializeField] Button questsButton = null;
        [SerializeField] Button mapButton = null;
        [SerializeField] Button saveButton = null;
        [SerializeField] Button loadButton = null;
        [SerializeField] Button optionsButton = null;
        [SerializeField] Button mainMenuButton = null;
        [SerializeField] Button quitButton = null;

        public event Action<MainMenuButtonType> onMenuButtonSelect;

        private void Start()
        {
            SetupButtons();
        }

        private void OnDisable()
        {
            foreach (Button button in GetMenuButtons())
            {
                button.GetComponent<MenuItemHighlighter>().ForceUnhighlight();
            }
        }

        private void SetupButtons()
        {
            characterButton.onClick.AddListener(() => OnCharacterButton());
            inventoryButton.onClick.AddListener(() => OnInventoryButton());
            questsButton.onClick.AddListener(() => OnQuestsButton());

            mapButton.onClick.AddListener(() => OnMapButton());
            saveButton.onClick.AddListener(() => OnSaveButton());
            loadButton.onClick.AddListener(() => OnLoadButton());
            optionsButton.onClick.AddListener(() => OnOptionsButton());
            mainMenuButton.onClick.AddListener(() => OnMainMenuButton());
            quitButton.onClick.AddListener(() => OnQuitButton());
        }

        private void OnCharacterButton()
        {
            onMenuButtonSelect(MainMenuButtonType.Character);
        }

        private void OnInventoryButton()
        {
            onMenuButtonSelect(MainMenuButtonType.Inventory);
        }

        private void OnQuestsButton()
        {
            onMenuButtonSelect(MainMenuButtonType.Quests);
        }

        private void OnMapButton()
        {
            //onMenuButtonSelect(MainMenuButtonType.Map);
        }

        private void OnSaveButton()
        {
            //FindObjectOfType<SavingWrapper>().Save();
        }

        private void OnLoadButton()
        {
            //FindObjectOfType<SavingWrapper>().Load();
        }

        private void OnOptionsButton()
        {
            //onMenuButtonSelect(MainMenuButtonType.Options);
        }

        private void OnMainMenuButton()
        {
            onMenuButtonSelect(MainMenuButtonType.MainMenu);
        }

        private void OnQuitButton()
        {
            Application.Quit();
        }

        private IEnumerable<Button> GetMenuButtons()
        {
            yield return characterButton;
            yield return inventoryButton;
            yield return questsButton;
            yield return mapButton;
            yield return optionsButton;
            yield return mainMenuButton;
            yield return quitButton;
        }
    }
}
