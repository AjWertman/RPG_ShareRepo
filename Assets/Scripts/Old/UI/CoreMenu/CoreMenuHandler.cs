using RPGProject.Combat;
using RPGProject.Core;
using System.Collections.Generic;
using UnityEngine;

public class CoreMenuHandler : MonoBehaviour
{
    [Header("Selection Menus")]
    [SerializeField] MainMenu coreMainMenu = null;
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

    public void ActivateCoreMainMenu(bool shouldActivate)
    {
        coreMainMenu.gameObject.SetActive(shouldActivate);
        currencyMenu.gameObject.SetActive(shouldActivate);
    }
    
    private void BackToMainMenu()
    {
        DeactivateAllMenus();
        ActivateCoreMainMenu(true);
    }

    private void OpenMenu(MainMenuButtonType mainMenuButtonType)
    {
        DeactivateAllMenus();

        if (mainMenuButtonType == MainMenuButtonType.Character)
        {
            OpenCharacterSelectMenu();
        }
        else if (mainMenuButtonType == MainMenuButtonType.Inventory)
        {
            OpenInventoryMenu();
        }
        else if (mainMenuButtonType == MainMenuButtonType.Quests)
        {
            OpenQuestsMenu();
        }
        else if (mainMenuButtonType == MainMenuButtonType.Map)
        {
            OpenMapMenu();
        }
        else if (mainMenuButtonType == MainMenuButtonType.Options)
        {
            OpenOptionsMenu();
        }
        else if (mainMenuButtonType == MainMenuButtonType.MainMenu)
        {
            DirectToMainMenu();
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

    private void OpenCharacterMenu(Unit character)
    {
        characterSelectMenu.gameObject.SetActive(false);

        TeamInfo newTeamInfo = playerTeam.GetTeamInfo(character);
        characterMenu.SetupCharacterMenu(character, newTeamInfo);
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

    public bool IsAnyMenuActive()
    {
        foreach(GameObject menu in GetAllMenus())
        {
            if (menu.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerable<GameObject> GetAllMenus()
    {
        yield return coreMainMenu.gameObject;
        yield return characterSelectMenu.gameObject;
        yield return characterMenu.gameObject;
        yield return questMenu.gameObject;
        yield return currencyMenu.gameObject;
    }
}
