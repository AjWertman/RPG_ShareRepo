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
        [SerializeField] PlayerMoveSelect playerMoveSelectMenu = null;
        [SerializeField] AbilitySelect abilitySelectMenu = null;
        [SerializeField] TargetSelect targetSelectMenu = null;

        List<Fighter> playerCombatants = new List<Fighter>();
        List<Fighter> enemyCombatants = new List<Fighter>();

        Fighter currentCombatantTurn = null;
        Fighter target = null;
        Fighter highlightedTarget = null;

        Ability selectedAbility = null;
        bool isCopy = false;

        Dictionary<Fighter, UnitUI> fighterUIDict = new Dictionary<Fighter, UnitUI>();
        Dictionary<BattleUIMenuKey, GameObject> menuGameObjects = new Dictionary<BattleUIMenuKey, GameObject>();
        BattleUIMenuKey activeMenuKey = BattleUIMenuKey.None;

        public event Action<Fighter, Ability> onPlayerMove;
        public event Action onEscape;
        public event Action onEndTurn;

        public void InitalizeBattleUIManager()
        {
            PopulateMenuGODict();
            InitalizeSelectionMenus();
            DeactivateAllMenus();
        }

        private void InitalizeSelectionMenus()
        {
            playerMoveSelectMenu.InitalizePlayerMoveSelectMenu();
            abilitySelectMenu.InitalizeAbilitySelectMenu();
            targetSelectMenu.InitalizeTargetSelectMenu();

            playerMoveSelectMenu.onPlayerMoveSelect += OnPlayerMoveSelect;

            abilitySelectMenu.onAbilitySelect += OnAbilitySelect;
            abilitySelectMenu.onBackButton += () => ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);

            targetSelectMenu.onTargetSelect += OnTargetSelect;
            targetSelectMenu.onTargetHighlight += HighlightTarget;
            targetSelectMenu.onTargetUnhighlight += UnhighlightTarget;
            targetSelectMenu.onBackButton += ActivateBattleUIMenu;

            battleHUD.onTurnOrderHighlight += HighlightTarget;
            battleHUD.onTurnOrderUnhighlight += UnhighlightTarget;
        }

        public void SetupUIManager(List<Fighter> _playerCombatants, List<Fighter> _enemyCombatants, List<Fighter> _turnOrder)
        {
            UpdateUnitLists(_playerCombatants, _enemyCombatants);
            SetCurrentCombatantTurn(_turnOrder[0]);

            battleHUD.UpdateTurnOrderUIItems(_turnOrder, currentCombatantTurn);
        }

        public void ActivateBattleUIMenu(BattleUIMenuKey _battleUIMenu)
        {
            DeactivateAllMenus();

            switch (_battleUIMenu)
            {
                case BattleUIMenuKey.PlayerMoveSelect:

                    ActivatePlayerMoveSelectMenu(true);
                    break;

                case BattleUIMenuKey.AbilitySelect:

                    ActivateAbilitySelectMenu(true);
                    break;

                case BattleUIMenuKey.TargetSelect:

                    ActivateTargetSelectMenu(true);
                    break;
            }

            activeMenuKey = _battleUIMenu;
        }

        private void OnPlayerMoveSelect(PlayerMoveType _playerMoveType)
        {
            switch (_playerMoveType)
            {
                case PlayerMoveType.Attack:

                    Ability basicAttack = currentCombatantTurn.GetUnitInfo().GetBasicAttack();
                    OnAbilitySelect(basicAttack);
                    break;

                case PlayerMoveType.AbilitySelect:

                    ActivateBattleUIMenu(BattleUIMenuKey.AbilitySelect);
                    break;

                case PlayerMoveType.ItemSelect:

                    ActivateBattleUIMenu(BattleUIMenuKey.ItemSelect);
                    break;

                case PlayerMoveType.Escape:

                    onEscape();
                    break;

                case PlayerMoveType.EndTurn:

                    onEndTurn();
                    break;
            }
        }

        public void ActivatePlayerMoveSelectMenu(bool _shouldActivate)
        {
            if (IsActivationObsolete(BattleUIMenuKey.PlayerMoveSelect, _shouldActivate)) return;

            playerMoveSelectMenu.gameObject.SetActive(_shouldActivate);
        }

        public void ActivateAbilitySelectMenu(bool _shouldActivate)
        {
            if (IsActivationObsolete(BattleUIMenuKey.AbilitySelect, _shouldActivate)) return;

            if (_shouldActivate)
            {;
                abilitySelectMenu.PopulateAbilitiesList(currentCombatantTurn, currentCombatantTurn.GetKnownAbilities());
            }
            else
            {
                abilitySelectMenu.ResetAbilitySelectMenu();
            }

            abilitySelectMenu.gameObject.SetActive(_shouldActivate);
        }

        public void ActivateTargetSelectMenu(bool _shouldActivate)
        {
            if (IsActivationObsolete(BattleUIMenuKey.TargetSelect, _shouldActivate)) return;

            if (_shouldActivate)
            {
                TargetingType targetingType = selectedAbility.GetTargetingType();
                targetSelectMenu.SetupTargetSelectMenu(activeMenuKey, targetingType);
            }
            else
            {
                targetSelectMenu.ResetTargetButtons();
            }

            targetSelectMenu.gameObject.SetActive(_shouldActivate);
        }

        public void OnAbilitySelect(Ability _ability)
        {
            selectedAbility = _ability;

            if (selectedAbility.CanTargetAll())
            {
                OnTargetSelect(null);
                return;
            }
          
            List<Fighter> targetTeam = new List<Fighter>();

            if (selectedAbility.GetTargetingType() == TargetingType.PlayersOnly)
            {
                targetTeam = playerCombatants;
            }
            else if(selectedAbility.GetTargetingType() == TargetingType.EnemiesOnly)
            {
                targetTeam = enemyCombatants;
            }
            else if (selectedAbility.GetTargetingType() == TargetingType.SelfOnly)
            {
                OnTargetSelect(currentCombatantTurn);
                return;
            }

            if (targetTeam.Count > 1)
            {
                ActivateBattleUIMenu(BattleUIMenuKey.TargetSelect);
            }
            else
            {
                OnTargetSelect(targetTeam[0]);
            }
        }

        public void OnTargetSelect(Fighter _target)
        {
            if(CombatAssistant.IsAlreadyEffected(selectedAbility.GetAbilityName(), _target.GetUnitStatus()))
            {
                StartCoroutine(battleHUD.ActivateCantUseAbilityUI("Target is already effected by that ability"));
                ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
                return;
            }
            onPlayerMove(_target, selectedAbility);

            ActivateUnitTurnUI(currentCombatantTurn, false);
            battleHUD.SetupUnitResourcesIndicator(null);
        }

        public void ExecuteNextTurn(List<Fighter> _turnOrder, Fighter _currentCombatantTurn)
        {
            SetCurrentCombatantTurn(_currentCombatantTurn);
            battleHUD.UpdateTurnOrderUIItems(_turnOrder, currentCombatantTurn);

            if (currentCombatantTurn.GetUnitInfo().IsPlayer())
            {
                ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
            }
            else
            {
                DeactivateAllMenus();
            }
        }
       
        public void HighlightTarget(Fighter _combatant)
        {
            if (_combatant == null) return;
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

            battleHUD.SetupUnitResourcesIndicator(null);
            highlightedTarget = null;
        }

        public void DeactivateAllMenus()
        {
            ActivatePlayerMoveSelectMenu(false);
            ActivateAbilitySelectMenu(false);
            ActivateTargetSelectMenu(false);
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

        public void SetTarget(Fighter _target)
        {
            target = _target;
        }

        public void UpdateUnitLists(List<Fighter> _playerUnits, List<Fighter> _enemyUnits)
        {
            playerCombatants = _playerUnits;
            enemyCombatants = _enemyUnits;

            UpdateUnitUIDict(_playerUnits, _enemyUnits);

            targetSelectMenu.UpdateUnitLists(playerCombatants, enemyCombatants);
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

            target = null;

            abilitySelectMenu.ResetAbilitySelectMenu();
            targetSelectMenu.ResetTargetSelectMenu();
            battleHUD.ResetTurnOrderUIItems();

            activeMenuKey = BattleUIMenuKey.None;
        }

        public BattleHUD GetBattleHUD()
        {
            return battleHUD;
        }

        public UnitUI GetUnitUI(Fighter _fighter)
        {
            return fighterUIDict[_fighter];
        }

        private void PopulateMenuGODict()
        {
            menuGameObjects[BattleUIMenuKey.PlayerMoveSelect] = playerMoveSelectMenu.gameObject;
            menuGameObjects[BattleUIMenuKey.AbilitySelect] = abilitySelectMenu.gameObject;
            menuGameObjects[BattleUIMenuKey.TargetSelect] = targetSelectMenu.gameObject;
        }

        private bool IsActivationObsolete(BattleUIMenuKey _battleUIMenuKey, bool _isActivating)
        {
            GameObject menuGO = menuGameObjects[_battleUIMenuKey];
            bool isObsolete = (menuGO.activeSelf == _isActivating);
            return isObsolete;
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

namespace RPGProject.UI
{
    public enum BattleUIMenuKey { None, PlayerMoveSelect, AbilitySelect, ItemSelect, TargetSelect }
}