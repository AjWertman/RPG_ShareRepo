using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleUIMenuKey { None, PlayerMoveSelect, AbilitySelect, TargetSelect}

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] BattleHUD battleHUD = null;
    [SerializeField] PlayerMoveSelect playerMoveSelectMenu = null;
    [SerializeField] AbilitySelect abilitySelectMenu = null;
    [SerializeField] TargetSelect targetSelectMenu = null;

    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();

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
        targetSelectMenu.onBack += ActivateBattleUIMenu;

        battleHUD.onTurnOrderHighlight += HighlightTarget;
        battleHUD.onTurnOrderUnhighlight += UnhighlightTarget;
    }

    public void SetupUIManager(List<BattleUnit> _playerUnits, List<BattleUnit> _enemyUnits, List<BattleUnit> _turnOrder)
    {
        UpdateBattleUnitLists(_playerUnits, new List<BattleUnit>(), _enemyUnits, new List<BattleUnit>());
        battleHUD.SetupBattleHUD(_turnOrder);
    }

    public void ActivateBattleUIMenu(BattleUIMenuKey _battleUIMenu)
    {
        DeactivateAllMenus();

        activeMenuKey = _battleUIMenu;

        switch (activeMenuKey)
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
    }

    private void OnPlayerMoveSelect(PlayerMoveType _playerMoveType)
    {
        switch (_playerMoveType)
        {
            case PlayerMoveType.Attack:

                //Remove AbilityButton from Attack button
                Ability basicAttack = currentBattleUnitTurn.GetBattleUnitInfo().GetBasicAttack();
                OnAbilitySelect(basicAttack);
                break;

            case PlayerMoveType.AbilitySelect:
        
                ActivateBattleUIMenu(BattleUIMenuKey.AbilitySelect);
                break;

            case PlayerMoveType.ItemSelect:
                //item select
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
            TargetingType targetingType = selectedAbility.targetingType;
            targetSelectMenu.SetupTargetSelectMenu(activeMenuKey, targetingType);
        }
        else
        {
            targetSelectMenu.ResetTargetSelectMenu();
        }

        targetSelectMenu.gameObject.SetActive(_shouldActivate);
    }

    public void OnAbilitySelect(Ability _ability)
    {
        selectedAbility = _ability;

        List<BattleUnit> targetTeam = targetSelectMenu.GetTargets(selectedAbility.targetingType);

        if(targetTeam.Count > 1)
        {
            targetSelectMenu.SetupTargetSelectMenu(activeMenuKey, selectedAbility.targetingType);
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

    public void ExecuteNextTurn(int _newTurnIndex)
    {
        battleHUD.SetupTurnOrderUIItems(_newTurnIndex);
    }

    public void HighlightTarget(BattleUnit _battleUnit)
    {
        if (_battleUnit == null) return;
        highlightedTarget = _battleUnit;

        highlightedTarget.ActivateUnitIndicatorUI(true);
        battleHUD.SetupUnitResourcesIndicator(highlightedTarget);
    }

    public void UnhighlightTarget()
    {
        if (highlightedTarget == null) return;

        if(highlightedTarget != currentBattleUnitTurn)
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

        activeMenuKey = BattleUIMenuKey.None;
    }

    public void SetCurrentBattleUnitTurn(BattleUnit _currentBattletUnitTurn)
    {
        currentBattleUnitTurn = _currentBattletUnitTurn;
    }

    public void SetSelectedAbility(Ability _selectedAbility)
    {
        selectedAbility = _selectedAbility;
    }

    public void SetTargetBattleUnit(BattleUnit _targetBattleUnit)
    {
        targetBattleUnit = _targetBattleUnit;
    }

    public void UpdateBattleUnitLists(List<BattleUnit> _playerUnits, List<BattleUnit> _deadPlayerUnits, List<BattleUnit> _enemyUnits, List<BattleUnit> _deadEnemyUnits)
    {
        playerUnits = _playerUnits;
        deadPlayerUnits = _deadPlayerUnits;

        enemyUnits = _enemyUnits;
        deadEnemyUnits = _deadEnemyUnits;

        targetSelectMenu.UpdateBattleUnitLists(playerUnits, deadPlayerUnits, enemyUnits, deadEnemyUnits);
    }

    public void ResetUIManager()
    {
        playerUnits.Clear();
        deadPlayerUnits.Clear();
        enemyUnits.Clear();
        deadEnemyUnits.Clear();

        currentBattleUnitTurn = null;
        selectedAbility = null;
        isCopy = false;

        targetBattleUnit = null;
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

    /// <summary>

    public void SetUILookAts(Transform lookTransform)
    {
        foreach (LookAtCam lookAtCam in FindObjectsOfType<LookAtCam>())
        {
            lookAtCam.LookAtCamTransform(lookTransform);      
        }
    }

    //public void OnPlayerMoveButton()
    //{
    //    TargetingType type = selectedAbility.targetingType;

    //    if (type == TargetingType.EnemysOnly)
    //    {
    //        if (enemyUnits.Count > 1 && !selectedAbility.canTargetAll)
    //        {
    //            ActivateTargetSelectCanvas();
    //        }
    //        else if (selectedAbility.canTargetAll)
    //        {
    //            onPlayerMove(null, selectedAbility);
    //        }
    //        else
    //        {
    //            onPlayerMove(enemyUnits[0], selectedAbility);
    //        }
    //    }
    //    else if (type == TargetingType.PlayersOnly)
    //    {
    //        //player implementation
    //    }
    //    else if (type == TargetingType.SelfOnly)
    //    {
    //        onPlayerMove(currentBattleUnit, selectedAbility);
    //    }
    //}

    //TargetSelect///////////////////////////////////////////////////////////////////////////////////////////

    //public void ActivateTargetSelectCanvas()
    //{
    //    TargetingType targetingType = selectedAbility.targetingType;

    //    if (targetingType == TargetingType.SelfOnly)
    //    {
    //        onPlayerMove(currentBattleUnit, selectedAbility);
    //        return;
    //    }
    //    else if (targetingType == TargetingType.EnemysOnly)
    //    {
    //        playerGroupButton.gameObject.SetActive(false);
    //        enemyGroupButton.gameObject.SetActive(true);

    //        OnGroupButtonSelect(false);
    //    }
    //    else if (targetingType == TargetingType.PlayersOnly)
    //    {
    //        enemyGroupButton.gameObject.SetActive(false);
    //        playerGroupButton.gameObject.SetActive(true);

    //        OnGroupButtonSelect(true);
    //    }
    //    else if (targetingType == TargetingType.Everyone)
    //    {
    //        enemyGroupButton.gameObject.SetActive(true);
    //        playerGroupButton.gameObject.SetActive(true);

    //        OnGroupButtonSelect(false);
    //    }

    //    targetSelectCanvas.SetActive(true);
    //}
}