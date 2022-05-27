using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] UnitResourcesIndicator unitResourcesIndicator = null;
    [SerializeField] Transform turnOrderContent = null;
    [SerializeField] GameObject turnOrderUIPrefab = null;

    TextMeshProUGUI cantCastText = null;

    int amountOfTurnOrderUIItems = 8;

    List<TurnOrderUIItem> turnOrderUIItems = new List<TurnOrderUIItem>();

    List<BattleUnit> currentTurnOrder = new List<BattleUnit>();

    public event Action<BattleUnit> onTurnOrderHighlight;
    public event Action onTurnOrderUnhighlight;

    private void Awake()
    {
        CreateTurnOrderUIItemPool();
    }

    private void CreateTurnOrderUIItemPool()
    {
        for (int i = 0; i < amountOfTurnOrderUIItems; i++)
        {
            GameObject turnOrderInstance = Instantiate(turnOrderUIPrefab, turnOrderContent);
            TurnOrderUIItem turnOrderUIItem = turnOrderInstance.GetComponent<TurnOrderUIItem>();

            turnOrderUIItem.onPointerEnter += OnTurnOrderHighlight;
            turnOrderUIItem.onPointerExit += OnTurnOrderUnhighlight;

            turnOrderUIItems.Add(turnOrderUIItem);

            turnOrderInstance.gameObject.SetActive(false);
        }
    }

    public void UpdateTurnOrderUIItems(List<BattleUnit> _currentTurnOrder, int _currentTurn)
    {
        List<BattleUnit> aliveUnits = GetAliveUnits(_currentTurnOrder);
        SetCurrentTurnOrder(aliveUnits);
        SetupTurnOrderUIItems(_currentTurn);
    }

    public void SetupTurnOrderUIItems(int _currentTurn)
    {
        List<BattleUnit> uiTurnOrder = GetUITurnOrder(_currentTurn);

        for (int i = 0; i < amountOfTurnOrderUIItems; i++)
        {
            BattleUnit battleUnit = uiTurnOrder[i];
            TurnOrderUIItem currentTurnOrderUIItem = turnOrderUIItems[i];

            currentTurnOrderUIItem.SetupTurnOrderUI(i, battleUnit);
            currentTurnOrderUIItem.gameObject.SetActive(true);
        }
    }

    public void SetupUnitResourcesIndicator(BattleUnit _battleUnit)
    {
        unitResourcesIndicator.SetupResourceIndicator(_battleUnit);

        bool isBattleUnitNull = (_battleUnit == null);
        unitResourcesIndicator.gameObject.SetActive(!isBattleUnitNull);
    }

    public void SetCurrentTurnOrder(List<BattleUnit> _turnOrder)
    {
        currentTurnOrder = _turnOrder;
    }

    private void OnTurnOrderHighlight(BattleUnit _battleUnit)
    {
        onTurnOrderHighlight(_battleUnit);
    }

    private void OnTurnOrderUnhighlight()
    {
        onTurnOrderUnhighlight();
    }
    
    public void ResetTurnOrderUIItems()
    {
        foreach (TurnOrderUIItem turnOrderUIItem in turnOrderUIItems)
        {
            turnOrderUIItem.ResetTurnOrderUIItem();
        }
    }

    public IEnumerator ActivateCantUseAbilityUI(string _reason)
    {
        yield return CantUseAbilityUI(_reason);
    }

    public IEnumerator CantUseAbilityUI(string _reason)
    {
        cantCastText.text = _reason;
        cantCastText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        cantCastText.text = "";
        cantCastText.gameObject.SetActive(false);
    }

    public List<BattleUnit> GetUITurnOrder(int _currentTurn)
    {
        List<BattleUnit> uiTurnOrder = new List<BattleUnit>();
        int index = _currentTurn;

        for (int i = 0; i < amountOfTurnOrderUIItems; i++)
        {
            BattleUnit unitTurn = currentTurnOrder[index];
            uiTurnOrder.Add(unitTurn);
            index = UpdateTurnOrderIndex(index);
        }

        return uiTurnOrder;
    }

    public List<BattleUnit> GetAliveUnits(List<BattleUnit> _allBattleUnits)
    {
        List<BattleUnit> aliveUnits = new List<BattleUnit>();

        foreach (BattleUnit battleUnit in _allBattleUnits)
        {
            if (!battleUnit.IsDead())
            {
                aliveUnits.Add(battleUnit);
            }

        }

        return aliveUnits;
    }

    public int UpdateTurnOrderIndex(int _currentIndex)
    {
        int updatedIndex = _currentIndex + 1;

        if(updatedIndex >= currentTurnOrder.Count)
        {
            updatedIndex = 0;
        }

        return updatedIndex;
    }
}
