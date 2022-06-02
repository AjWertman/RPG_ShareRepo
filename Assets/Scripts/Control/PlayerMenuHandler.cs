using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.UI;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class PlayerMenuHandler : MonoBehaviour
    {
        [Header("Selection Menus")]
        [SerializeField] CoreMainMenu coreMainMenu = null;
        [SerializeField] CharacterSelectMenu characterSelectMenu = null;

        [Header("Menus")]
        [SerializeField] CharacterMenu characterMenu = null;
        [SerializeField] QuestMenu questMenu = null;

        [Header("Other UI")]
        [SerializeField] CurrencyUI currencyMenu = null;

        PlayerTeam playerTeam = null;

        private void Start()
        {
            coreMainMenu.onMenuButtonSelect += OpenMenu;

            characterSelectMenu.onCharacterSelect += OpenCharacterMenu;
            characterSelectMenu.onBackToMainCoreMenu += BackToMainMenu;

            characterMenu.onBackToCharacterSelect += BackToCharacterSelectMenu;

            playerTeam = FindObjectOfType<PlayerTeam>();
            characterSelectMenu.SetupCharacterSelectMenu(playerTeam.GetPlayerTeam());

            DeactivateAllMenus();
        }

        public void ActivateCoreMainMenu(bool _shouldActivate)
        {
            DeactivateAllMenus();
            coreMainMenu.gameObject.SetActive(_shouldActivate);
            currencyMenu.gameObject.SetActive(_shouldActivate);
        }

        private void BackToMainMenu()
        {
            DeactivateAllMenus();
            ActivateCoreMainMenu(true);
        }

        private void OpenMenu(MainMenuButtonType _mainMenuButtonType)
        {
            DeactivateAllMenus();

            switch (_mainMenuButtonType)
            {
                case MainMenuButtonType.Character:
                    OpenCharacterSelectMenu();
                    break;

                case MainMenuButtonType.Inventory:
                    OpenInventoryMenu();
                    break;

                case MainMenuButtonType.Quests:
                    OpenQuestsMenu();
                    break;

                case MainMenuButtonType.Map:
                    OpenMapMenu();
                    break;

                case MainMenuButtonType.Options:
                    OpenOptionsMenu();
                    break;

                case MainMenuButtonType.MainMenu:
                    DirectToMainMenu();
                    break;

            }
        }

        private void OpenCharacterSelectMenu()
        {
            characterSelectMenu.gameObject.SetActive(true);
        }

        private void BackToCharacterSelectMenu()
        {
            characterMenu.gameObject.SetActive(false);
            characterSelectMenu.gameObject.SetActive(true);
        }

        private void OpenCharacterMenu(Unit _character)
        {
            characterSelectMenu.gameObject.SetActive(false);

            TeamInfo newTeamInfo = playerTeam.GetTeamInfo(_character);
            characterMenu.SetupCharacterMenu(_character, newTeamInfo);
            characterMenu.gameObject.SetActive(true);
        }

        private void OpenInventoryMenu()
        {

        }

        private void OpenQuestsMenu()
        {
            questMenu.ActivateQuestsPage();
            questMenu.gameObject.SetActive(true);
        }

        private void OpenMapMenu()
        {

        }

        private void OpenOptionsMenu()
        {

        }

        private void DirectToMainMenu()
        {
            FindObjectOfType<SceneManagerScript>().LoadMainMenu();
        }

        public void DeactivateAllMenus()
        {
            foreach (GameObject menu in GetAllMenus())
            {
                menu.SetActive(false);
            }
        }

        public IEnumerable<GameObject> GetAllMenus()
        {
            yield return coreMainMenu.gameObject;
            yield return characterSelectMenu.gameObject;
            yield return characterMenu.gameObject;
            yield return questMenu.gameObject;
            yield return currencyMenu.gameObject;
        }
    }
}
