using RPGProject.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public enum BattleUIMenuKey { None, PlayerMoveSelect, AbilitySelect, ItemSelect, TargetSelect }

    public class BattleUIManager : MonoBehaviour
    {
        [SerializeField] BattleHUD battleHUD = null;
        [SerializeField] PlayerMoveSelect playerMoveSelectMenu = null;
        [SerializeField] AbilitySelect abilitySelectMenu = null;
        [SerializeField] TargetSelect targetSelectMenu = null;

        List<BattleUnit> playerUnits = new List<BattleUnit>();
        List<BattleUnit> enemyUnits = new List<BattleUnit>();

        BattleUnit currentBattleUnitTurn = null;
        BattleUnit targetBattleUnit = null;
        BattleUnit highlightedTarget = null;

        Ability selectedAbility = null;
        bool isCopy = false;

        Dictionary<BattleUIMenuKey, GameObject> menuGameObjects = new Dictionary<BattleUIMenuKey, GameObject>();
        BattleUIMenuKey activeMenuKey = BattleUIMenuKey.None;

        public event Action<BattleUnit, Ability> onPlayerMove;
        public event Action onEscape;

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

        public void SetupUIManager(List<BattleUnit> _playerUnits, List<BattleUnit> _enemyUnits, List<BattleUnit> _turnOrder)
        {
            SetCurrentBattleUnitTurn(_turnOrder[0]);
            UpdateBattleUnitLists(_playerUnits, _enemyUnits);
            battleHUD.UpdateTurnOrderUIItems(_turnOrder, currentBattleUnitTurn);
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

                    Ability basicAttack = currentBattleUnitTurn.GetBattleUnitInfo().GetBasicAttack();
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
            {
                abilitySelectMenu.PopulateAbilitiesList(currentBattleUnitTurn);
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

            List<BattleUnit> targetTeam = new List<BattleUnit>();

            if (selectedAbility.GetTargetingType() == TargetingType.PlayersOnly)
            {
                targetTeam = playerUnits;
            }
            else
            {
                targetTeam = enemyUnits;
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

        public void OnTargetSelect(BattleUnit _target)
        {
            onPlayerMove(_target, selectedAbility);

            battleHUD.SetupUnitResourcesIndicator(null);
            _target.ActivateUnitIndicatorUI(false);
        }

        public void ExecuteNextTurn(List<BattleUnit> _turnOrder, BattleUnit _currentBattleUnitTurn)
        {
            SetCurrentBattleUnitTurn(_currentBattleUnitTurn);
            battleHUD.UpdateTurnOrderUIItems(_turnOrder, _currentBattleUnitTurn);

            if (currentBattleUnitTurn.GetBattleUnitInfo().IsPlayer())
            {
                ActivateBattleUIMenu(BattleUIMenuKey.PlayerMoveSelect);
            }
        }

        public void HighlightTarget(BattleUnit _battleUnit)
        {
            if (_battleUnit == null) return;
            highlightedTarget = _battleUnit;

            highlightedTarget.ActivateUnitIndicatorUI(true);
            battleHUD.SetupUnitResourcesIndicator(highlightedTarget);
            
            //Refactor
            //Resources UI above head activate
        }

        public void UnhighlightTarget()
        {
            if (highlightedTarget == null) return;

            if (highlightedTarget != currentBattleUnitTurn)
            {
                highlightedTarget.ActivateUnitIndicatorUI(false);
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

        public void SetCurrentBattleUnitTurn(BattleUnit _currentBattletUnitTurn)
        {
            if(currentBattleUnitTurn != null)
            {
                currentBattleUnitTurn.ActivateUnitIndicatorUI(false);
            }

            currentBattleUnitTurn = _currentBattletUnitTurn;
            currentBattleUnitTurn.ActivateUnitIndicatorUI(true);
        }

        public void SetSelectedAbility(Ability _selectedAbility)
        {
            selectedAbility = _selectedAbility;
        }

        public void SetTargetBattleUnit(BattleUnit _targetBattleUnit)
        {
            targetBattleUnit = _targetBattleUnit;
        }

        public void UpdateBattleUnitLists(List<BattleUnit> _playerUnits, List<BattleUnit> _enemyUnits)
        {
            playerUnits = _playerUnits;
            enemyUnits = _enemyUnits;

            targetSelectMenu.UpdateBattleUnitLists(playerUnits, enemyUnits);
        }

        public void ResetUIManager()
        {
            playerUnits.Clear();
            enemyUnits.Clear();

            currentBattleUnitTurn = null;
            selectedAbility = null;
            isCopy = false;

            targetBattleUnit = null;

            abilitySelectMenu.ResetAbilitySelectMenu();
            targetSelectMenu.ResetTargetSelectMenu();
            battleHUD.ResetTurnOrderUIItems();

            activeMenuKey = BattleUIMenuKey.None;
        }

        public BattleHUD GetBattleHUD()
        {
            return battleHUD;
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