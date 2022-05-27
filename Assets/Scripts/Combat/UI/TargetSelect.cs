using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
    public event Action<BattleUIMenuKey> onBackButton;

    public void InitalizeTargetSelectMenu()
    {
        CreateTargetButtonsPool();

        enemyGroupButton.onClick.AddListener(() => OnGroupButtonSelect(false));
        playerGroupButton.onClick.AddListener(() => OnGroupButtonSelect(true));

        backButton.onClick.AddListener(OnBackButton);

        DeactivateGroupButtons();
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
        ResetTargetButtons();
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

        SetupGroupButtons(_targetingType);

        bool isPlayerTarget = (_targetingType == TargetingType.PlayersOnly);
        PopulateTargetButtons(isPlayerTarget);
    }

    public void PopulateTargetButtons(bool _isPlayer)
    {
        ResetTargetButtons();

        List<BattleUnit> targets = GetTargets(_isPlayer);

        foreach (BattleUnit target in targets)
        {
            TargetButton targetButton = GetAvailableTargetButton();
            targetButton.SetupTargetButton(target);
            targetButtons[targetButton] = true;
            targetButton.gameObject.SetActive(true);
        }
    }

    public void SetupGroupButtons(TargetingType _targetingType)
    {
        DeactivateGroupButtons();
        switch (_targetingType)
        {
            case TargetingType.PlayersOnly:

                playerGroupButton.gameObject.SetActive(true);
                playerGroupButton.interactable = false;

                enemyGroupButton.gameObject.SetActive(false);
                break;

            case TargetingType.EnemiesOnly:

                enemyGroupButton.gameObject.SetActive(true);
                enemyGroupButton.interactable = false;

                playerGroupButton.gameObject.SetActive(false);
                break;

            case TargetingType.Everyone:

                //have a bool to decide whether spell is friendly or not and start with that button.
                //IE:Player has a holy spell that would be used to heal living beings, however, it does extra damage to undead
                enemyGroupButton.gameObject.SetActive(true);
                enemyGroupButton.interactable = false;

                playerGroupButton.gameObject.SetActive(true);
                playerGroupButton.interactable = true;
                break;
        }
    }

    private void OnGroupButtonSelect(bool _isPlayer)
    {
        if (_isPlayer)
        {
            playerGroupButton.interactable = false;
            enemyGroupButton.interactable = true;
            PopulateTargetButtons(true);
        }
        else
        {
            enemyGroupButton.interactable = false;
            playerGroupButton.interactable = true;
            PopulateTargetButtons(false);
        }
    }

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

    public void ResetTargetButtons()
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

    private void DeactivateGroupButtons()
    {
        playerGroupButton.interactable = false;
        enemyGroupButton.interactable = false;
        playerGroupButton.gameObject.SetActive(false);
        enemyGroupButton.gameObject.SetActive(false);
    }

    public void OnBackButton()
    {
        onBackButton(previousPageKey);
    }

    public List<BattleUnit> GetTargets(bool _isPlayer)
    {
        List<BattleUnit> targets = new List<BattleUnit>();

        foreach(BattleUnit battleUnit in GetTeamList(_isPlayer))
        {
            if (!battleUnit.IsDead())
            {
                targets.Add(battleUnit);
            }
        }
              
        return targets;
    }

    public List<BattleUnit> GetTeamList(bool _isPlayer)
    {
        List<BattleUnit> teamList = new List<BattleUnit>();  

        if (_isPlayer)
        {
            teamList = playerUnits;
        }
        else
        {
            teamList = enemyUnits;
        }

        return teamList;
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
