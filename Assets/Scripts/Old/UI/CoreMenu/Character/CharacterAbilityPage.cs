using RPGProject.Combat;
using RPGProject.Core;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class CharacterAbilityPage : MonoBehaviour
    {
        [SerializeField] Image classImage = null;
        [SerializeField] TextMeshProUGUI classText = null;
        [SerializeField] TextMeshProUGUI classDescription = null;
        [SerializeField] TextMeshProUGUI subClassText = null;

        [SerializeField] GameObject abilityButtonPrefab = null;
        [SerializeField] Transform abilityButtonContainer = null;

        [SerializeField] TextMeshProUGUI abilityDescription = null;

        List<AbilityUIButton> abilityUIButtons = new List<AbilityUIButton>();

        private void Awake()
        {
            CreateAbilityButtonsPool();
        }

        private void CreateAbilityButtonsPool()
        {
            for (int i = 0; i < 15; i++)
            {
                GameObject abilityButtonInstance = Instantiate(abilityButtonPrefab, abilityButtonContainer);
                AbilityUIButton abilityUIButton = abilityButtonInstance.GetComponent<AbilityUIButton>();

                abilityUIButtons.Add(abilityUIButton);

                abilityUIButton.onMouseEnter += ActivateAbilityTooltip;
                abilityUIButton.onMouseExit += DeactivateAbilityTooltip;
            }

            ResetAbilityButtons();
        }

        public void SetupAbilityPage(Ability[] _abilities)
        {
            //Refactor - playable character

            PopulateAbilityList(_abilities);
            DeactivateAbilityTooltip();
        }

        private void PopulateAbilityList(Ability[] _abilities)
        {
            foreach(Ability ability in _abilities)
            {
                AbilityUIButton abilityUIButton = GetAvailableAbilityUIButton();
                abilityUIButton.SetupAbilityButton(ability);
                abilityUIButton.gameObject.SetActive(true);
            }
        }

        public void ResetAbilityButtons()
        {
            foreach(AbilityUIButton abilityButton in abilityUIButtons)
            {
                abilityButton.ResetSpellButton();
                abilityButton.gameObject.SetActive(false);
            }
        }

        private void ActivateAbilityTooltip(Ability _ability)
        {
            abilityDescription.text = _ability.GetDescription();
        }

        private void DeactivateAbilityTooltip()
        {
            abilityDescription.text = "*No Ability Selected*";
        }

        private AbilityUIButton GetAvailableAbilityUIButton()
        {
            AbilityUIButton availableAbilityUIButton = null;

            foreach (AbilityUIButton abilityUIButton in abilityUIButtons)
            {
                if(abilityUIButton.GetAbility() == null)
                {
                    availableAbilityUIButton = abilityUIButton;
                    break;
                }
            }

            return availableAbilityUIButton;
        }
    }
}
