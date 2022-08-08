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
        [SerializeField] GameObject inventoryMenu = null;

        [Header("Other UI")]
        [SerializeField] CurrencyUI currencyMenu = null;

        PlayerTeamManager playerTeam = null;

        public void InitializeMenu()
        {
            coreMainMenu.onMenuButtonSelect += OpenMenu;

            questMenu.onBackButton += ()=> ActivateCoreMainMenu(true);

            characterSelectMenu.onCharacterSelect += OpenCharacterMenu;
            characterSelectMenu.onBackToMainCoreMenu += BackToMainMenu;

            characterMenu.onBackToCharacterSelect += BackToCharacterSelectMenu;

            playerTeam = FindObjectOfType<PlayerTeamManager>();

            questMenu.InitializeMenu();

            DeactivateAllMenus();
        }

        public void ActivateCoreMainMenu(bool _shouldActivate)
        {
            DeactivateAllMenus();
            coreMainMenu.gameObject.SetActive(_shouldActivate);
            currencyMenu.gameObject.SetActive(_shouldActivate);
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
            yield return inventoryMenu.gameObject;
            yield return currencyMenu.gameObject;
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
            characterSelectMenu.SetupCharacterSelectMenu(playerTeam.GetPlayableCharacters());
            characterSelectMenu.gameObject.SetActive(true);
        }

        private void BackToCharacterSelectMenu()
        {
            characterMenu.gameObject.SetActive(false);
            characterSelectMenu.gameObject.SetActive(true);
        }

        private void OpenCharacterMenu(PlayableCharacter _character)
        {
            PlayerKey playerKey = _character.playerKey;
            TeamInfo teamInfo = playerTeam.GetTeamInfo(playerKey);
            Unit unit = playerTeam.GetUnit(playerKey);

            characterSelectMenu.gameObject.SetActive(false);

            TeamInfo newTeamInfo = playerTeam.GetTeamInfo(_character.playerKey);
            characterMenu.SetupCharacterMenu(_character, unit, teamInfo.level, newTeamInfo.unitResources);
            characterMenu.gameObject.SetActive(true);
        }

        private void OpenInventoryMenu()
        {

            inventoryMenu.gameObject.SetActive(true);
        }

        private void OpenQuestsMenu()
        {
            questMenu.ActivateQuestsPage(false);
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
    }
}
