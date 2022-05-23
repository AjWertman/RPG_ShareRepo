using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("Player Move UI")]
    [SerializeField] GameObject playerMoveCanvas = null;
    [SerializeField] Button attackButton = null;
    [SerializeField] Button spellSelectButton = null;
    [SerializeField] Button itemButton = null;
    [SerializeField] Button runAwayButton = null;

    [Header("Spell Select UI")]
    [SerializeField] GameObject spellSelectCanvas = null;
    [SerializeField] RectTransform contentRectTransform = null;
    [SerializeField] GameObject abilityButtonObject = null;
    [SerializeField] SpellTooltip spellTooltip = null;
    [SerializeField] Button spellToPlayerMoveButton = null;

    [Header("Target Select UI")]
    [SerializeField] GameObject targetSelectCanvas = null;
    [SerializeField] GameObject targetButtonPrefab = null;
    [SerializeField] RectTransform targetButtonLocation = null;
    [SerializeField] Button playerGroupButton = null;
    [SerializeField] Button enemyGroupButton = null;
    [SerializeField] Button targetBackButton = null;

    [Header("Other UI")]
    [SerializeField] GameObject hudObject = null;
    [SerializeField] GameObject cantCastCanvas = null;
    [SerializeField] UnitResourcesIndicator unitResourcesIndicator = null;
    [SerializeField] Transform turnOrderUI = null;
    [SerializeField] GameObject turnOrderUIItem = null;

    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();

    BattleUnit currentBattleUnit = null;

    List<TurnOrderUIItem> turnOrderUIItems = new List<TurnOrderUIItem>(); 

    Ability selectedAbility = null;
    bool isCopy = false;
    BattleUnit selectedBattleUnit = null;

    List<Ability> copySpellList = new List<Ability>();

    public event Action<BattleUnit, Ability> onPlayerMove;
    public event Action onEscape;

    //Initialization///////////////////////////////////////////////////////////////////////////////////////////

    public void SetUpBattleUI(List<BattleUnit> _playerUnits, List<BattleUnit> _enemyUnits)
    {         
        playerUnits = _playerUnits;
        enemyUnits = _enemyUnits;

        SetupPlayerMoveUI();
        SetupSpellSelectUI();
        SetupTargetSelectUI();
        spellTooltip.gameObject.SetActive(false);
    }

    public void DeactivateAllUI()
    {
        DeactivatePlayerMoveCanvas();
        DeactivateAbilitySelectCanvas();
        DeactivateTargetSelectCanvas();
        DeactivateSpellTooltip();
    }

    //Create pool here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void SetupTurnOrderUI(List<BattleUnit> _turnOrder)
    {
        for (int i = 0; i < _turnOrder.Count; i++)
        {
            GameObject turnOrderInstance = Instantiate(turnOrderUIItem, turnOrderUI);
            TurnOrderUIItem turnOrderItem = turnOrderInstance.GetComponent<TurnOrderUIItem>();
            BattleUnit currentUnit = _turnOrder[i];
            turnOrderItem.SetupTurnOrderUI(i, currentUnit);

            turnOrderUIItems.Add(turnOrderItem);
        }
    }

    public void RotateTurnOrder()
    {
        HandleDeadUnits();
        DestroyOldCreateNew();
        turnOrderUIItems[0].SetSize(0);
    }

    private void DestroyOldCreateNew()
    {
        TurnOrderUIItem firstTOItem = turnOrderUIItems[0];
        BattleUnit firstUnit = firstTOItem.GetBattleUnit();

        turnOrderUIItems.Remove(firstTOItem);
        Destroy(firstTOItem.gameObject);
        DecrementTurnOrderIndex();

        GameObject lastTOObject = Instantiate(turnOrderUIItem, turnOrderUI);
        TurnOrderUIItem lastTOItem = lastTOObject.GetComponent<TurnOrderUIItem>();

        turnOrderUIItems.Add(lastTOItem);
        lastTOItem.SetupTurnOrderUI(turnOrderUIItems.Count - 1, firstUnit);
    }
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    private void HandleDeadUnits()
    {
        List<TurnOrderUIItem> uiItemsToRemove = new List<TurnOrderUIItem>();
       
        foreach (TurnOrderUIItem turnOrderUIItem in turnOrderUIItems)
        {
            if (turnOrderUIItem.GetBattleUnit().IsDead())
            {
                uiItemsToRemove.Add(turnOrderUIItem);
            }
        }

        foreach (TurnOrderUIItem itemToRemove in uiItemsToRemove)
        {
            int indexToRemove = itemToRemove.GetIndex();
            turnOrderUIItems.Remove(itemToRemove);
            Destroy(itemToRemove.gameObject);

            for (int i = 0; i < turnOrderUIItems.Count; i++)
            {
                if (i > indexToRemove)
                {
                    turnOrderUIItems[i].DecrementIndex();
                }
            }
        }      
    }

    private void DecrementTurnOrderIndex()
    {
        foreach (TurnOrderUIItem turnOrderUIItem in turnOrderUIItems)
        {
            turnOrderUIItem.DecrementIndex();
        }
    }

    public void ActivateUnitResourcesUI(BattleUnit _battleUnit)
    {     
        unitResourcesIndicator.SetupResourceIndicator(_battleUnit);     
        unitResourcesIndicator.gameObject.SetActive(true);
    }

    public void DeactivateUnitResourcesUI()
    {
        unitResourcesIndicator.gameObject.SetActive(false);
    }

    public void SetUILookAts(Transform lookTransform)
    {
        foreach (LookAtCam lookAtCam in FindObjectsOfType<LookAtCam>())
        {
            lookAtCam.LookAtCamTransform(lookTransform);      
        }
    }


    private void SetupPlayerMoveUI()
    {
        attackButton.onClick.AddListener(() =>  SetAbility(attackButton.GetComponent<AbilityButton>().GetAssignedAbility(), false));
        attackButton.onClick.AddListener(() => DeactivatePlayerMoveCanvas());
        attackButton.onClick.AddListener(() => OnPlayerMoveButton());

        spellSelectButton.onClick.AddListener(() => DeactivatePlayerMoveCanvas());
        spellSelectButton.onClick.AddListener(() => ActivateAbilitySelectCanvas());

        itemButton.onClick.AddListener(() => SetAbility(itemButton.GetComponent<AbilityButton>().GetAssignedAbility(), false));
        itemButton.onClick.AddListener(() => DeactivatePlayerMoveCanvas());
        itemButton.onClick.AddListener(() => OnPlayerMoveButton());

        runAwayButton.onClick.AddListener(Escape);
    }

    private void SetupSpellSelectUI()
    {
        spellToPlayerMoveButton.onClick.AddListener(() => DeactivateAbilitySelectCanvas());
        spellToPlayerMoveButton.onClick.AddListener(() => ActivatePlayerMoveCanvas());
    }

    private void SetupTargetSelectUI()
    {
        enemyGroupButton.onClick.AddListener(() => OnGroupButtonSelect(false));
        playerGroupButton.onClick.AddListener(() => OnGroupButtonSelect(true));

        targetBackButton.onClick.AddListener(() => DeactivateTargetSelectCanvas());
        targetBackButton.onClick.AddListener(() => ActivatePlayerMoveCanvas());
    }

    //PlayerMove///////////////////////////////////////////////////////////////////////////////////////////
    public void ActivatePlayerMoveCanvas()
    {
        Ability basicAttack = currentBattleUnit.GetBattleUnitInfo().GetBasicAttack();
        attackButton.GetComponent<AbilityButton>().SetAssignedAbility(basicAttack);

        if (!GetComponent<TutorialUIHandler>())
        {
            if (currentBattleUnit.GetFighter().IsSilenced() || currentBattleUnit.GetFighter().HasSubstitute())
            {
                spellSelectButton.interactable = false;
            }
            else
            {
                spellSelectButton.interactable = true;
            }
        }

        playerMoveCanvas.SetActive(true);
    }

    public void OnPlayerMoveButton()
    {
        TargetingType type = selectedAbility.targetingType;

        if (type == TargetingType.EnemysOnly)
        {
            if (enemyUnits.Count > 1 && !selectedAbility.canTargetAll)
            {
                ActivateTargetSelectCanvas();
            }
            else if (selectedAbility.canTargetAll)
            {
                onPlayerMove(null, selectedAbility);
            }
            else
            {
                onPlayerMove(enemyUnits[0], selectedAbility);
            }
        }
        else if(type == TargetingType.PlayersOnly)
        {
            //player implementation
        }
        else if (type == TargetingType.SelfOnly)
        {
            onPlayerMove(currentBattleUnit, selectedAbility);
        }
    }

    public void DeactivatePlayerMoveCanvas()
    {
        playerMoveCanvas.SetActive(false);
    }

    public void Escape()
    {
        onEscape();
    }

    public void SetCurrentBattleUnit(BattleUnit _currentUnit)
    {
        currentBattleUnit = _currentUnit;
    }

    public IEnumerator CantCastSpellUI(string _reason)
    {  
        cantCastCanvas.GetComponentInChildren<TextMeshProUGUI>().text = _reason;
        cantCastCanvas.SetActive(true);

        DeactivateTargetSelectCanvas();
        ActivatePlayerMoveCanvas();

        yield return new WaitForSeconds(1.5f);

        cantCastCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "";
        cantCastCanvas.SetActive(false);
    }

    //SpellSelect///////////////////////////////////////////////////////////////////////////////////////////

    public void PopulateAbilitiesList(Ability[] _abilities)
    {
        ClearSpellList();

        int unitLevel = currentBattleUnit.GetBattleUnitInfo().GetUnitLevel();
        foreach (Ability ability in _abilities)
        {
            if (unitLevel < ability.requiredLevel) continue;

            GameObject newButtonInstance = Instantiate(abilityButtonObject, contentRectTransform);
            Button button = newButtonInstance.GetComponent<Button>();
            AbilityButton abilityButton = newButtonInstance.GetComponent<AbilityButton>();

            string cantCastReason = CantCast(ability);
            if (cantCastReason != "")
            {
                button.interactable = false;
            }

            abilityButton.SetAssignedAbility(ability);
            abilityButton.SetCantCastReason(cantCastReason);
          
            abilityButton.onPointerEnter += ActivateSpellTooltip;
            abilityButton.onPointerExit += DeactivateSpellTooltip;

            button.onClick.AddListener(() => DeactivateSpellTooltip());

            if (ability.spellType != SpellType.RenCopy)
            {
                button.onClick.AddListener(() => SetAbility(abilityButton.GetAssignedAbility(), false));
                button.onClick.AddListener(() => DeactivateAbilitySelectCanvas());
                button.onClick.AddListener(() => OnPlayerMoveButton());   
            }
            else
            {
                if (copySpellList.Count > 0)
                {
                    button.interactable = true;
                    button.onClick.AddListener(PopulateRenCopySpellList);
                }
                else
                {
                    button.interactable = false;
                }
            }

            button.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;
            button.GetComponentInChildren<TextMeshProUGUI>().color = ability.textColor;
            button.image.color = abilityButton.GetAssignedAbility().buttonColor;
        }
    }


    public void PopulateRenCopySpellList()
    {       
        ClearSpellList();
        foreach(Ability ability in copySpellList)
        {
            GameObject newButtonInstance = Instantiate(abilityButtonObject, contentRectTransform);
            Button button = newButtonInstance.GetComponent<Button>();
            AbilityButton abilityButton = newButtonInstance.GetComponent<AbilityButton>();

            string cantCastReason = CantCast(ability);
            if (cantCastReason != "")
            {
                button.interactable = false;
            }

            abilityButton.SetAssignedAbility(ability);
            abilityButton.SetCantCastReason(cantCastReason);

            abilityButton.onPointerEnter += ActivateSpellTooltip;
            abilityButton.onPointerExit += DeactivateSpellTooltip;

            button.onClick.AddListener(() => SetAbility(abilityButton.GetAssignedAbility(),true));
            button.onClick.AddListener(() => DeactivateAbilitySelectCanvas());
            button.onClick.AddListener(() => DeactivateSpellTooltip());
            button.onClick.AddListener(()=> OnPlayerMoveButton());

            button.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;
            button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            button.image.color = Color.black;
        }
    }

    public void SetAbility(Ability _abilityToSet, bool _isCopy)
    {      
        selectedAbility = _abilityToSet;
        isCopy = _isCopy;
    }

    private string CantCast(Ability ability)
    {
        if (!currentBattleUnit.HasEnoughMana(ability.manaCost))
        {
            return "Does not have enough Mana";
        }

        if(ability.spellType == SpellType.RenCopy)
        {
            if(copySpellList.Count== 0)
            {
                return "No copyable spells cast";
            }
        }

        if(ability.targetingType == TargetingType.SelfOnly)
        {
            if(ability.spellType == SpellType.Static)
            {
                StaticSpell staticSpell = ability.spellPrefab.GetComponent<StaticSpell>();
                if (staticSpell.GetStaticSpellType() == StaticSpellType.PhysicalReflector)
                {
                    if(currentBattleUnit.GetFighter().GetPhysicalReflectionDamage() > 0)
                    {
                        return "Spell already active";
                    }
                }
                else if (staticSpell.GetStaticSpellType() == StaticSpellType.SpellReflector)
                {
                    if (currentBattleUnit.GetFighter().IsReflectingSpells())
                    {
                        return "Spell already active";
                    }
                }
            }
        }

        return "";
    }

    public void ActivateAbilitySelectCanvas()
    {
        Ability[] abilities = currentBattleUnit.GetBattleUnitInfo().GetAbilities();
        PopulateAbilitiesList(abilities);
        spellSelectCanvas.SetActive(true);
    }

    public void DeactivateAbilitySelectCanvas()
    {
        spellSelectCanvas.SetActive(false);
    }

    private void ActivateSpellTooltip(Ability _ability, string _cantCastReason)
    {
        if (_ability.abilityType == AbilityType.Physical) return;
        spellTooltip.SetupTooltip(_ability);
        spellTooltip.gameObject.SetActive(true);

        if (_cantCastReason == "") return;
        spellTooltip.SetupCantCast(_cantCastReason);
    }

    private void DeactivateSpellTooltip()
    {
        spellTooltip.gameObject.SetActive(false);
    }

    public void UpdateRenCopyList(List<Ability> renList)
    {
        ClearRenList();
        foreach(Ability renCopy in renList)
        {
            copySpellList.Add(renCopy);
        }
    }

    public void ClearRenList()
    {
        if (copySpellList.Count <= 0) return;
        copySpellList.Clear();    
    }

    public void ClearSpellList()
    {
        foreach(RectTransform spellButton in contentRectTransform)
        {
            Destroy(spellButton.gameObject);
        }
    }

    //TargetSelect///////////////////////////////////////////////////////////////////////////////////////////

    public void ActivateTargetSelectCanvas()
    {
        TargetingType targetingType = selectedAbility.targetingType;

        if (targetingType == TargetingType.SelfOnly)
        {
            onPlayerMove(currentBattleUnit, selectedAbility);
            return;
        }
        else if (targetingType == TargetingType.EnemysOnly)
        {
            playerGroupButton.gameObject.SetActive(false);
            enemyGroupButton.gameObject.SetActive(true);

            OnGroupButtonSelect(false);
        }
        else if (targetingType == TargetingType.PlayersOnly)
        {
            enemyGroupButton.gameObject.SetActive(false);
            playerGroupButton.gameObject.SetActive(true);

            OnGroupButtonSelect(true);
        }
        else if (targetingType == TargetingType.Everyone)
        {
            enemyGroupButton.gameObject.SetActive(true);
            playerGroupButton.gameObject.SetActive(true);

            OnGroupButtonSelect(false);
        }

        targetSelectCanvas.SetActive(true);
    }

    public void PopulateEnemyButtons()
    {
        ClearTargetButtons();
        for (int i = 0; i < enemyUnits.Count; i++)
        {
            GameObject targetButtonInstance = Instantiate(targetButtonPrefab, targetButtonLocation);
            Button button = targetButtonInstance.GetComponent<Button>();
            TargetButton targetButton = targetButtonInstance.GetComponent<TargetButton>();

            targetButton.SetupTargetButton(enemyUnits[i]);
            OnTargetButtonClick(button, targetButton);
        }
    }

    public void PopulatePlayerButtons()
    {
        ClearTargetButtons();
        for (int i = 0; i < playerUnits.Count; i++)
        {
            GameObject targetButtonInstance = Instantiate(targetButtonPrefab, targetButtonLocation);
            Button button = targetButtonInstance.GetComponent<Button>();
            TargetButton targetButton = targetButtonInstance.GetComponent<TargetButton>();
   
            targetButton.SetupTargetButton(playerUnits[i]);
            OnTargetButtonClick(button, targetButton);
        }
    }

    private void OnTargetButtonClick(Button _button, TargetButton _targetButton)
    {
        _button.onClick.AddListener(() => SetTarget(_targetButton.GetTarget()));
        _button.onClick.AddListener(() => _targetButton.GetTarget().ActivateUnitIndicatorUI(false));
        _button.onClick.AddListener(() => DeactivateTargetSelectCanvas());
        _button.onClick.AddListener(() => onPlayerMove(selectedBattleUnit, selectedAbility));
        _button.onClick.AddListener(() => DeactivateUnitResourcesUI());
    }

    public void DeactivateTargetSelectCanvas()
    {
        targetSelectCanvas.SetActive(false);
    }

    private void ClearTargetButtons()
    {
        foreach(RectTransform button in targetButtonLocation)
        {
            Destroy(button.gameObject);
        }
    }

    private void OnGroupButtonSelect(bool _isPlayer)
    {
        if (_isPlayer)
        {
            playerGroupButton.interactable = false;
            enemyGroupButton.interactable = true;
            PopulatePlayerButtons();
        }
        else
        {
            enemyGroupButton.interactable = false;
            playerGroupButton.interactable = true;
            PopulateEnemyButtons();
        }
    }

    public void SetTarget(BattleUnit _battleUnit)
    {
        selectedBattleUnit = _battleUnit;
    }

    public void UpdateUnitLists(List<BattleUnit> _playerUnits, List<BattleUnit> _deadPlayerUnits, List<BattleUnit> _enemyUnits, List<BattleUnit> _deadEnemyUnits)
    {
        playerUnits = _playerUnits;
        deadPlayerUnits = _deadPlayerUnits;

        enemyUnits = _enemyUnits;
        deadEnemyUnits = _deadEnemyUnits;
    }

    public void ResetUIManager()
    {
        playerUnits.Clear();
        deadPlayerUnits.Clear();
        enemyUnits.Clear();
        deadEnemyUnits.Clear();
        currentBattleUnit = null;
        selectedAbility = null;
        isCopy = false;
        selectedBattleUnit = null;
        copySpellList.Clear();
    }
}
