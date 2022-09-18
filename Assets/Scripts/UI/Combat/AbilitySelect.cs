using RPGProject.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// The menu that the player can select one of their character's abilities from.
    /// </summary>
    public class AbilitySelect : MonoBehaviour
    {
        [SerializeField] RectTransform contentRectTransform = null;
        [SerializeField] GameObject abilityButtonPrefab = null;
        [SerializeField] int amountOfAbilityButtonsToPool = 15;

        [SerializeField] SpellTooltip spellTooltip = null;
        [SerializeField] Button backButton = null;

        Dictionary<AbilityButton, bool> abilityButtons = new Dictionary<AbilityButton, bool>();
        Dictionary<Ability, string> unusableAbilities = new Dictionary<Ability, string>();

        List<Ability> copySpellList = new List<Ability>();

        public event Action<Ability> onAbilitySelect;
        public event Action onBackButton;

        public void InitalizeAbilitySelectMenu()
        {
            CreateAbilityButtonsPool();
            spellTooltip.gameObject.SetActive(false);

            backButton.onClick.AddListener(() => OnAbilitySelect(null));
        }

        private void CreateAbilityButtonsPool()
        {
            for (int i = 0; i < amountOfAbilityButtonsToPool; i++)
            {
                GameObject abilityButtonInstance = Instantiate(abilityButtonPrefab, contentRectTransform);
                AbilityButton abilityButton = abilityButtonInstance.GetComponent<AbilityButton>();

                abilityButton.InitalizeAbilityButton();

                abilityButton.onSelect += OnAbilitySelect;
                abilityButton.onPointerEnter += ActivateSpellTooltip;
                abilityButton.onPointerExit += DeactivateSpellTooltip;

                abilityButton.GetButton().onClick.AddListener(DeactivateSpellTooltip);

                abilityButtons.Add(abilityButton, false);
                abilityButton.gameObject.SetActive(false);
            }
        }

        public void PopulateAbilitiesList(Fighter _caster, List<Ability> _abilities)
        {
            ResetAbilitiesList();
            unusableAbilities.Clear();

            foreach (Ability ability in _abilities)
            {
                if (ability == _caster.unitInfo.basicAttack) continue;
                AbilityButton availableAbilityButton = GetAvailableAbilityButton();

                bool isCopyAbility = (ability.abilityType== AbilityType.Copy);

                string cantUseAbilityReason = CombatAssistant.CanUseAbilityCheck(_caster, ability);

                availableAbilityButton.SetAssignedAbility(ability);
                abilityButtons[availableAbilityButton] = true;

                if (isCopyAbility)
                {
                    //SetupCopyAbilityButton(availableAbilityButton);
                }

                if (cantUseAbilityReason != "")
                {
                    unusableAbilities.Add(ability, cantUseAbilityReason);
                    availableAbilityButton.GetButton().interactable = false;
                }

                availableAbilityButton.gameObject.SetActive(true);
            }
        }

        private void OnAbilitySelect(Ability _ability)
        {
            onAbilitySelect(_ability);
        }

        //private void SetupCopyAbilityButton(AbilityButton _abilityButton)
        //{
        //    if (copySpellList.Count > 0)
        //    {
        //        _abilityButton.GetButton().interactable = true; 

        //        //HaveCopyAbilitiesList????
        //        //button.onClick.AddListener(PopulateRenCopySpellList);
        //    }
        //    else
        //    {
        //        _abilityButton.GetButton().interactable = false;
        //    }
        //}

        public void ResetAbilitySelectMenu()
        {
            ResetAbilitiesList();
            unusableAbilities.Clear();
            copySpellList.Clear();
        }

        private void ResetAbilitiesList()
        {
            List<AbilityButton> abilityButtonsToReset = new List<AbilityButton>();

            foreach (AbilityButton abilityButton in GetAllActiveAbilityButtons())
            {
                abilityButtonsToReset.Add(abilityButton);
            }

            foreach (AbilityButton abilityButton in abilityButtonsToReset)
            {
                abilityButton.ResetAbilityButton();
                abilityButtons[abilityButton] = false;
                abilityButton.gameObject.SetActive(false);
            }
        }

        private void ActivateSpellTooltip(Ability _ability)
        {
            spellTooltip.SetupTooltip(_ability);

            if (unusableAbilities.ContainsKey(_ability))
            {
                string cantUseAbilityReason = unusableAbilities[_ability];
                spellTooltip.SetupCantCast(cantUseAbilityReason);
            }

            spellTooltip.gameObject.SetActive(true);
        }

        private void DeactivateSpellTooltip()
        {
            spellTooltip.gameObject.SetActive(false);
        }

        private AbilityButton GetAvailableAbilityButton()
        {
            AbilityButton availableAbilityButton = null;

            foreach (AbilityButton abilityButton in abilityButtons.Keys)
            {
                if (abilityButtons[abilityButton] == false)
                {
                    availableAbilityButton = abilityButton;
                    break;
                }
            }

            return availableAbilityButton;
        }

        private IEnumerable<AbilityButton> GetAllActiveAbilityButtons()
        {
            foreach (AbilityButton abilityButton in abilityButtons.Keys)
            {
                if (abilityButtons[abilityButton])
                {
                    yield return abilityButton;
                }
            }
        }
    }
    //public void UpdateRenCopyList(List<Ability> renList)
    //{
    //    ClearRenList();
    //    foreach(Ability renCopy in renList)
    //    {
    //        copySpellList.Add(renCopy);
    //    }
    //}

    //public void ClearRenList()
    //{
    //    if (copySpellList.Count <= 0) return;
    //    copySpellList.Clear();    
    //}

    //public void PopulateRenCopySpellList()
    //{       
    //    ClearSpellList();
    //    foreach(Ability ability in copySpellList)
    //    {
    //        GameObject newButtonInstance = Instantiate(abilityButtonObject, contentRectTransform);
    //        Button button = newButtonInstance.GetComponent<Button>();
    //        AbilityButton abilityButton = newButtonInstance.GetComponent<AbilityButton>();

    //        string cantCastReason = CantCast(ability);
    //        if (cantCastReason != "")
    //        {
    //            button.interactable = false;
    //        }

    //        abilityButton.SetAssignedAbility(ability);
    //        abilityButton.SetCantCastReason(cantCastReason);

    //        abilityButton.onPointerEnter += ActivateSpellTooltip;
    //        abilityButton.onPointerExit += DeactivateSpellTooltip;

    //        button.onClick.AddListener(() => SetAbility(abilityButton.GetAssignedAbility(),true));
    //        button.onClick.AddListener(() => DeactivateAbilitySelectCanvas());
    //        button.onClick.AddListener(() => DeactivateSpellTooltip());
    //        button.onClick.AddListener(()=> OnPlayerMoveButton());

    //        button.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;
    //        button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    //        button.image.color = Color.black;
    //    }
    //}
}