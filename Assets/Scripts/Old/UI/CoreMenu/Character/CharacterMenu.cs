using RPGProject.Combat;
using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class CharacterMenu : MonoBehaviour
    {
        [SerializeField] Button backTab = null;
        [SerializeField] Button characterTab = null;
        [SerializeField] Button equipmentTab = null;
        [SerializeField] Button abilityTab = null;
        [SerializeField] Button statsTab = null;

        [SerializeField] CharacterAboutPage characterPage = null;
        [SerializeField] GameObject equipmentPage = null;
        [SerializeField] CharacterAbilityPage abilityPage = null;
        [SerializeField] CharacterStatsPage statsPage = null;

        PlayerTeam playerTeam = null;

        public event Action onBackToCharacterSelect;

        private void Start()
        {
            backTab.onClick.AddListener(() => onBackToCharacterSelect());
            characterTab.onClick.AddListener(() => ActivatePage(characterTab, characterPage.gameObject));
            equipmentTab.onClick.AddListener(() => ActivatePage(equipmentTab, equipmentPage.gameObject));
            abilityTab.onClick.AddListener(() => ActivatePage(abilityTab, abilityPage.gameObject));
            statsTab.onClick.AddListener(() => ActivatePage(statsTab, statsPage.gameObject));

            DeactivateAllPages();
        }

        public void SetupCharacterMenu(Unit _character, TeamInfo _teamInfo)
        {
            characterPage.SetupCharacterPage(_character, _teamInfo);
            abilityPage.SetupAbilityPage(_character);
            statsPage.SetupStatPageUI(_character, _teamInfo);
            ActivatePage(characterTab, characterPage.gameObject);
        }

        private void ActivatePage(Button _selectedTab, GameObject _pageToActive)
        {
            if (_pageToActive.activeSelf) return;
            SetSelectedTabColor(_selectedTab);

            DeactivateAllPages();
            _pageToActive.SetActive(true);
        }

        private void SetSelectedTabColor(Button _selectedTab)
        {
            foreach (Button tab in GetTabs())
            {
                tab.interactable = true;
            }
            _selectedTab.interactable = false;
        }

        private IEnumerable<Button> GetTabs()
        {
            yield return characterTab;
            yield return equipmentTab;
            yield return abilityTab;
            yield return statsTab;
        }

        private void DeactivateAllPages()
        {
            characterPage.gameObject.SetActive(false);
            equipmentPage.gameObject.SetActive(false);
            abilityPage.gameObject.SetActive(false);
            statsPage.gameObject.SetActive(false);
        }
    }
}