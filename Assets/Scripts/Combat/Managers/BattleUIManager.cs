using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    bool isRenCopy = false;
    BattleUnit selectedBattleUnit = null;

    List<Ability> renCopySpellList = new List<Ability>();

    public event Action<BattleUnit, Ability, bool> onPlayerMove;
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
        DeactivateSpellSelectCanvas();
        DeactivateTargetSelectCanvas();
        DeactivateSpellTooltip();

        hudObject.SetActive(false);
    }

    public void SetupTurnOrderUI(List<BattleUnit> turnOrder)
    {
        for (int i = 0; i < turnOrder.Count; i++)
        {
            GameObject turnOrderInstance = Instantiate(turnOrderUIItem, turnOrderUI);
            TurnOrderUIItem turnOrderItem = turnOrderInstance.GetComponent<TurnOrderUIItem>();
            BattleUnit currentUnit = turnOrder[i];
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

    public void ActivateUnitResourcesUI(BattleUnit unit)
    {     
        unitResourcesIndicator.SetupResourceIndicator(unit);     
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
        spellSelectButton.onClick.AddListener(() => ActivateSpellSelectCanvas());

        itemButton.onClick.AddListener(() => SetAbility(itemButton.GetComponent<AbilityButton>().GetAssignedAbility(), false));
        itemButton.onClick.AddListener(() => DeactivatePlayerMoveCanvas());
        itemButton.onClick.AddListener(() => OnPlayerMoveButton());

        runAwayButton.onClick.AddListener(Escape);
    }

    private void SetupSpellSelectUI()
    {
        spellToPlayerMoveButton.onClick.AddListener(() => DeactivateSpellSelectCanvas());
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
        attackButton.GetComponent<AbilityButton>().SetAssignedAbility(currentBattleUnit.GetBasicAttack());

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
                onPlayerMove(null, selectedAbility, isRenCopy);
            }
            else
            {
                onPlayerMove(enemyUnits[0], selectedAbility, isRenCopy);
            }
        }
        else if(type == TargetingType.PlayersOnly)
        {
            //player implementation
        }
        else if (type == TargetingType.SelfOnly)
        {
            onPlayerMove(currentBattleUnit, selectedAbility, false);
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

    public void SetCurrentBattleUnit(BattleUnit currentUnit)
    {
        currentBattleUnit = currentUnit;
    }

    public IEnumerator CantCastSpellUI(string reason)
    {  
        cantCastCanvas.GetComponentInChildren<TextMeshProUGUI>().text = reason;
        cantCastCanvas.SetActive(true);

        DeactivateTargetSelectCanvas();
        ActivatePlayerMoveCanvas();

        yield return new WaitForSeconds(1.5f);

        cantCastCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "";
        cantCastCanvas.SetActive(false);
    }

    //SpellSelect///////////////////////////////////////////////////////////////////////////////////////////

    public void PopulateSpellList(Ability[] abilities)
    {
        ClearSpellList();
        foreach (Ability ability in abilities)
        {
            if (currentBattleUnit.GetUnitLevel() < ability.requiredLevel) continue;

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
                button.onClick.AddListener(() => DeactivateSpellSelectCanvas());
                button.onClick.AddListener(() => OnPlayerMoveButton());   
            }
            else
            {
                if (renCopySpellList.Count > 0)
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
        foreach(Ability ability in renCopySpellList)
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
            button.onClick.AddListener(() => DeactivateSpellSelectCanvas());
            button.onClick.AddListener(() => DeactivateSpellTooltip());
            button.onClick.AddListener(()=> OnPlayerMoveButton());

            button.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;
            button.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            button.image.color = Color.black;
        }
    }

    public void SetAbility(Ability abilityToSet, bool _isRenCopy)
    {      
        selectedAbility = abilityToSet;
        isRenCopy = _isRenCopy;
    }

    private string CantCast(Ability ability)
    {
        if (!currentBattleUnit.HasEnoughSoulWell(ability.soulWellCost))
        {
            return "Does not have enough Soul Well";
        }

        if(ability.spellType == SpellType.RenCopy)
        {
            if(renCopySpellList.Count== 0)
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

    public void ActivateSpellSelectCanvas()
    {
        PopulateSpellList(currentBattleUnit.GetSpells());
        spellSelectCanvas.SetActive(true);
    }

    public void DeactivateSpellSelectCanvas()
    {
        spellSelectCanvas.SetActive(false);
    }

    private void ActivateSpellTooltip(Ability ability, string cantCastReason)
    {
        if (ability.abilityType == AbilityType.Physical) return;
        spellTooltip.SetupTooltip(ability);
        spellTooltip.gameObject.SetActive(true);

        if (cantCastReason == "") return;
        spellTooltip.SetupCantCast(cantCastReason);
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
            renCopySpellList.Add(renCopy);
        }
    }

    public void ClearRenList()
    {
        if (renCopySpellList.Count <= 0) return;
        renCopySpellList.Clear();    
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
            onPlayerMove(currentBattleUnit, selectedAbility, isRenCopy);
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

    private void OnTargetButtonClick(Button button, TargetButton targetButton)
    {
        button.onClick.AddListener(() => SetTarget(targetButton.GetTarget()));
        button.onClick.AddListener(() => targetButton.GetTarget().ActivateUnitIndicatorUI(false));
        button.onClick.AddListener(() => DeactivateTargetSelectCanvas());
        button.onClick.AddListener(() => onPlayerMove(selectedBattleUnit, selectedAbility, isRenCopy));
        button.onClick.AddListener(() => DeactivateUnitResourcesUI());
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

    private void OnGroupButtonSelect(bool isPlayer)
    {
        if (isPlayer)
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

    public void SetTarget(BattleUnit battleUnitToSet)
    {
        selectedBattleUnit = battleUnitToSet;
    }

    public void UpdateUnitLists(List<BattleUnit> _playerUnits, List<BattleUnit> _deadPlayerUnits, List<BattleUnit> _enemyUnits, List<BattleUnit> _deadEnemyUnits)
    {
        playerUnits = _playerUnits;
        deadPlayerUnits = _deadPlayerUnits;

        enemyUnits = _enemyUnits;
        deadEnemyUnits = _deadEnemyUnits;
    }   
}
