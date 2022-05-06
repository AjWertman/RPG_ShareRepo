using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    [SerializeField] Button backTab = null;
    [SerializeField] Button characterTab = null;
    [SerializeField] Button equipmentTab = null;
    [SerializeField] Button spellsTab = null;
    [SerializeField] Button statsTab = null;

    [SerializeField] CharacterAboutPage characterPage = null;
    [SerializeField] GameObject equipmentPage = null;
    [SerializeField] CharacterSpellPage spellsPage = null;
    [SerializeField] CharacterStatsPage statsPage = null;

    PlayerTeam playerTeam = null;

    public event Action onBackToCharacterSelect;

    private void Start()
    {
        backTab.onClick.AddListener(() => onBackToCharacterSelect());
        characterTab.onClick.AddListener(() => ActivatePage(characterTab, characterPage.gameObject));
        equipmentTab.onClick.AddListener(() => ActivatePage(equipmentTab, equipmentPage.gameObject));
        spellsTab.onClick.AddListener(() => ActivatePage(spellsTab, spellsPage.gameObject));
        statsTab.onClick.AddListener(() => ActivatePage(statsTab, statsPage.gameObject));

        DeactivateAllPages();
    }

    public void SetupCharacterMenu(Character character, TeamInfo teamInfo)
    {
        characterPage.SetupCharacterPage(character, teamInfo);
        spellsPage.SetupSpellPage(character);        
        statsPage.SetupStatPageUI(character, teamInfo);
        ActivatePage(characterTab, characterPage.gameObject);
    }

    private void ActivatePage(Button selectedTab, GameObject pageToActive)
    {
        if (pageToActive.activeSelf) return;
        SetSelectedTabColor(selectedTab);

        DeactivateAllPages();
        pageToActive.SetActive(true);
    }

    private void SetSelectedTabColor(Button selectedTab)
    {
        foreach (Button tab in GetTabs())
        {
            tab.interactable = true;
        }
        selectedTab.interactable = false;
    }

    private IEnumerable<Button> GetTabs()
    {
        yield return characterTab;
        yield return equipmentTab;
        yield return spellsTab;
        yield return statsTab;
    }

    private void DeactivateAllPages()
    {
        characterPage.gameObject.SetActive(false);
        equipmentPage.gameObject.SetActive(false);
        spellsPage.gameObject.SetActive(false);
        statsPage.gameObject.SetActive(false);
    }
}
