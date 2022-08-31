using RPGProject.Combat;
using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// The menu that contains useful information about a specified character.
    /// </summary>
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

        public void SetupCharacterMenu(PlayableCharacter _character, Unit _unit, int _level ,UnitResources _unitResources)
        {
            characterPage.SetupCharacterPage(_character, _level);
            abilityPage.SetupAbilityPage(_unit.abilities);
            statsPage.SetupStatPageUI(_character, _level, _unit.stats, _unitResources);
            ActivatePage(characterTab, characterPage.gameObject);
        }

        private void ActivatePage(Button _selectedTab, GameObject _pageToActive)
        {
            if (_pageToActive.activeSelf) return;

            DeactivateAllPages();
            SetSelectedTabColor(_selectedTab);
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

        private void DeactivateAllPages()
        {
            characterPage.gameObject.SetActive(false);
            equipmentPage.gameObject.SetActive(false);
            abilityPage.gameObject.SetActive(false);
            statsPage.gameObject.SetActive(false);
        }

        private IEnumerable<Button> GetTabs()
        {
            yield return characterTab;
            yield return equipmentTab;
            yield return abilityTab;
            yield return statsTab;
        }
    }
}