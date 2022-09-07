using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class BattleUIManager : MonoBehaviour
    {
        [SerializeField] BattleHUD battleHUD = null;
        [SerializeField] AbilitySelect abilitySelectMenu = null;

        public Fighter highlightedTarget = null;
        public bool isUIHighlight = false;

        public bool isSelectingAbility = false;

        List<Fighter> playerCombatants = new List<Fighter>();
        List<Fighter> enemyCombatants = new List<Fighter>();

        Fighter currentCombatantTurn = null;

        Ability selectedAbility = null;
        bool isCopy = false;

        Dictionary<Fighter, UnitUI> fighterUIDict = new Dictionary<Fighter, UnitUI>();

        public event Action onEscape;
        public event Action onEndTurn;

        public event Action<Ability> onAbilitySelect;

        public void InitalizeBattleUIManager()
        {
            InitalizeSelectionMenus();
            abilitySelectMenu.gameObject.SetActive(false);
        }

        private void InitalizeSelectionMenus()
        {
            abilitySelectMenu.InitalizeAbilitySelectMenu();
            abilitySelectMenu.onAbilitySelect += OnAbilitySelect;

            battleHUD.onFighterHighlight += HighlightTarget;
            battleHUD.onFighterUnhighlight += UnhighlightTarget;

            abilitySelectMenu.gameObject.SetActive(false);
        }

        public void SetupUIManager(List<Fighter> _playerCombatants, List<Fighter> _enemyCombatants, List<Fighter> _turnOrder)
        {
            UpdateUnitLists(_playerCombatants, _enemyCombatants);
            SetCurrentCombatantTurn(_turnOrder[0]);

            battleHUD.UpdateTurnOrderUIItems(_turnOrder, currentCombatantTurn);
            battleHUD.SetupTeammemberIndicators(_playerCombatants);
        }

        public void OnAbilitySelectKey()
        {
            bool shouldActivate = !abilitySelectMenu.gameObject.activeSelf;

            if (shouldActivate) ActivateAbilitySelectMenu();
            else DeactivateAbilitySelectMenu();
        }

        public void ActivateAbilitySelectMenu()
        {
            if (abilitySelectMenu.gameObject.activeSelf) return;
            abilitySelectMenu.PopulateAbilitiesList(currentCombatantTurn, currentCombatantTurn.GetKnownAbilities());
            abilitySelectMenu.gameObject.SetActive(true);
            isSelectingAbility = true;
        }

        public void DeactivateAbilitySelectMenu()
        {
            if (!abilitySelectMenu.gameObject.activeSelf) return;
            abilitySelectMenu.ResetAbilitySelectMenu();
            abilitySelectMenu.gameObject.SetActive(false);
            isSelectingAbility = false;
        }

        public void OnAbilitySelect(Ability _ability)
        {
            selectedAbility = _ability;
            isSelectingAbility = false;
            DeactivateAbilitySelectMenu();
            onAbilitySelect(selectedAbility);
        }

        public void ExecuteNextTurn(List<Fighter> _turnOrder, Fighter _currentCombatantTurn)
        {
            SetCurrentCombatantTurn(_currentCombatantTurn);
            battleHUD.UpdateTurnOrderUIItems(_turnOrder, currentCombatantTurn);
        }
       
        public void HighlightTarget(Fighter _combatant, bool _isUIHighlight)
        {
            if (_combatant == null) return;
            if (highlightedTarget != null && highlightedTarget != _combatant) UnhighlightTarget();
            if (highlightedTarget != null && highlightedTarget ==_combatant) return;

            isUIHighlight = _isUIHighlight;
            highlightedTarget = _combatant;

            UnitUI unitUI = GetUnitUI(highlightedTarget);

            unitUI.ActivateUnitIndicator(true);
            unitUI.ActivateResourceSliders(true);
            battleHUD.SetupUnitResourcesIndicator(highlightedTarget);
        }

        public void UnhighlightTarget()
        {
            if (highlightedTarget == null) return;
            UnitUI unitUI = GetUnitUI(highlightedTarget);
            if (highlightedTarget != currentCombatantTurn)
            {
                unitUI.ActivateUnitIndicator(false);
                unitUI.ActivateResourceSliders(false);
            }

            if (highlightedTarget.unitInfo.isPlayer) battleHUD.GetTeamMemberIndicator(highlightedTarget).HideIndicator(false);

            battleHUD.GetSelectedTargetIndicator().DeactivateIndicator();
            highlightedTarget = null;
            isUIHighlight = false;
        }

        public void SetCurrentCombatantTurn(Fighter _currentCombatantTurn)
        {
            currentCombatantTurn = _currentCombatantTurn;
            ActivateUnitTurnUI(currentCombatantTurn, true);
        }

        public void ActivateUnitTurnUI(Fighter _fighter, bool _shouldActivate)
        {
            UnitUI unitUI = GetUnitUI(_fighter);
            if (unitUI == null) return;

            unitUI.ActivateResourceSliders(_shouldActivate);
            unitUI.ActivateUnitIndicator(_shouldActivate);
        }

        public void SetSelectedAbility(Ability _selectedAbility)
        {
            selectedAbility = _selectedAbility;
        }

        public void UpdateUnitLists(List<Fighter> _playerUnits, List<Fighter> _enemyUnits)
        {
            playerCombatants = _playerUnits;
            enemyCombatants = _enemyUnits;

            UpdateUnitUIDict(_playerUnits, _enemyUnits);
        }

        private void UpdateUnitUIDict(List<Fighter> _playerUnits, List<Fighter> _enemyUnits)
        {
            fighterUIDict.Clear();
            List<Fighter> allFighters = new List<Fighter>();

            foreach(Fighter fighter in _playerUnits)
            {
                if (allFighters.Contains(fighter)) continue;
                allFighters.Add(fighter);
            }
            foreach (Fighter fighter in _enemyUnits)
            {
                if (allFighters.Contains(fighter)) continue;
                allFighters.Add(fighter);
            }

            foreach(Fighter fighter in allFighters)
            {
                if (fighterUIDict.ContainsKey(fighter)) continue;

                UnitUI unitUI = fighter.GetComponent<UnitUI>();
                fighterUIDict.Add(fighter, unitUI);
            }
        }

        public void ResetUIManager()
        {
            UnhighlightTarget();
            
            playerCombatants.Clear();
            enemyCombatants.Clear();

            currentCombatantTurn = null;
            selectedAbility = null;
            isCopy = false;

            abilitySelectMenu.ResetAbilitySelectMenu();
            battleHUD.ResetTurnOrderUIItems();
        }

        public BattleHUD GetBattleHUD()
        {
            return battleHUD;
        }

        public UnitUI GetUnitUI(Fighter _fighter)
        {
            return fighterUIDict[_fighter];
        }

        public void SetUILookAts(Transform _lookTransform)
        {
            LookAtCam[] lookAtCams = FindObjectsOfType<LookAtCam>();
            foreach (LookAtCam lookAtCam in lookAtCams)
            {
                lookAtCam.LookAtCamTransform(_lookTransform);
            }
        }
    }
}