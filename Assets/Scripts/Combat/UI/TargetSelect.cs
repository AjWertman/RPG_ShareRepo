using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TargetSelectKey { None, Players, Enemies, All}

public class TargetSelect : MonoBehaviour
{
    [SerializeField] GameObject targetButtonPrefab = null;
    [SerializeField] RectTransform contentRectTransform = null;

    [SerializeField] Button playerGroupButton = null;
    [SerializeField] Button enemyGroupButton = null;
    [SerializeField] Button backButton = null;

    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();

    Dictionary<TargetButton, bool> targetButtons = new Dictionary<TargetButton, bool>();

    BattleUIMenuKey previousPageKey = BattleUIMenuKey.None;

    public event Action<BattleUnit> onTargetSelect;
    public event Action<BattleUnit> onTargetHighlight;
    public event Action onTargetUnhighlight;
    public event Action<BattleUIMenuKey> onBack;

    public void InitalizeTargetSelectMenu()
    {
        CreateTargetButtonsPool();

        //enemyGroupButton.onClick.AddListener(() => OnGroupButtonSelect(false));
        //playerGroupButton.onClick.AddListener(() => OnGroupButtonSelect(true));

        backButton.onClick.AddListener(() => onBack(previousPageKey));
    }

    private void CreateTargetButtonsPool()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject targetButtonInstance = Instantiate(targetButtonPrefab, contentRectTransform);
            TargetButton targetButton = targetButtonInstance.GetComponent<TargetButton>();

            targetButton.onSelect += OnTargetSelect;
     
            targetButton.onPointerEnter += OnTargetHighlight;
            targetButton.onPointerExit += OnTargetUnhighlight;

            targetButtons.Add(targetButton, false);
            targetButton.gameObject.SetActive(false);
        }
    }

    private void OnTargetSelect(BattleUnit _battleUnit)
    {
        onTargetSelect(_battleUnit);
    }

    private void OnTargetHighlight(BattleUnit _battleUnit)
    {
        onTargetHighlight(_battleUnit);
    }

    private void OnTargetUnhighlight()
    {
        onTargetUnhighlight();
    }

    public void SetupTargetSelectMenu(BattleUIMenuKey _previousMenuKey, TargetingType _targetingType)
    {
        previousPageKey = _previousMenuKey;
        PopulateTargetButtons(_targetingType);
    }

    public void PopulateTargetButtons(TargetingType _targetingType)
    {
        ResetTargetButtons();
        //set group button

        foreach(BattleUnit target in GetTargets(_targetingType))
        {
            TargetButton targetButton = GetAvailableTargetButton();
            targetButton.SetupTargetButton(target);
            targetButtons[targetButton] = true;
            targetButton.gameObject.SetActive(true);
        }
    }

    //private void OnGroupButtonSelect(bool _isPlayer)
    //{
    //    if (_isPlayer)
    //    {
    //        playerGroupButton.interactable = false;
    //        enemyGroupButton.interactable = true;
    //        PopulatePlayerButtons();
    //    }
    //    else
    //    {
    //        enemyGroupButton.interactable = false;
    //        playerGroupButton.interactable = true;
    //        PopulateEnemyButtons();
    //    }
    //}

    public void UpdateBattleUnitLists(List<BattleUnit> _playerUnits, List<BattleUnit> _deadPlayerUnits, List<BattleUnit> _enemyUnits, List<BattleUnit> _deadEnemyUnits)
    {
        playerUnits = _playerUnits;
        deadPlayerUnits = _deadPlayerUnits;

        enemyUnits = _enemyUnits;
        deadEnemyUnits = _deadEnemyUnits;
    }

    public void ResetTargetSelectMenu()
    {
        previousPageKey = BattleUIMenuKey.None;
        playerUnits.Clear();
        enemyUnits.Clear();
        deadPlayerUnits.Clear();
        deadEnemyUnits.Clear();

        ResetTargetButtons();
    }

    private void ResetTargetButtons()
    {
        List<TargetButton> activeTargetButtons = GetActiveTargetButtons();
        if (activeTargetButtons.Count <= 0) return;

        List<TargetButton> dictionaryUpdateList = new List<TargetButton>();

        foreach(TargetButton targetButton in activeTargetButtons)
        {
            targetButton.ResetTargetButton();   
            dictionaryUpdateList.Add(targetButton);
            targetButton.gameObject.SetActive(false);
        }

        foreach(TargetButton targetButton in dictionaryUpdateList)
        {
            targetButtons[targetButton] = false;
        }
    }

    public List<BattleUnit> GetTargets(TargetingType _targetingType)
    {
        List<BattleUnit> targetableBattleUnits = new List<BattleUnit>();

        TargetSelectKey targetsType = GetTargetsType(_targetingType);

        if (targetsType == TargetSelectKey.Players)
        {
            targetableBattleUnits = playerUnits;
        }
        else
        {
            targetableBattleUnits = enemyUnits;
        }

        return targetableBattleUnits;
    }

    private TargetSelectKey GetTargetsType(TargetingType _targetingType)
    {
        TargetSelectKey targetsType = TargetSelectKey.None;

        if (_targetingType == TargetingType.Everyone || _targetingType == TargetingType.EnemiesOnly)
        {
            targetsType = TargetSelectKey.Enemies;
        }
        else if (_targetingType == TargetingType.PlayersOnly)
        {
            targetsType = TargetSelectKey.Players;
        }

        return targetsType;
    }

    private TargetButton GetAvailableTargetButton()
    {
        TargetButton availableTargetButton = null;

        foreach (TargetButton targetButton in targetButtons.Keys)
        {

            if (targetButtons[targetButton] == false)
            {
                availableTargetButton = targetButton;
                break;
            }
        }

        return availableTargetButton;
    }
   
    private List<TargetButton> GetActiveTargetButtons()
    {
        List<TargetButton> activeTargetButtons = new List<TargetButton>();

        foreach(TargetButton targetButton in targetButtons.Keys)
        {
            if(targetButtons[targetButton])
            {
                activeTargetButtons.Add(targetButton);
            }
        }

        return activeTargetButtons;
    }
}
