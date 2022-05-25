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

    private void OnTurnOrderHighlight(BattleUnit _battleUnit)
    {
        onTurnOrderHighlight(_battleUnit);
    }

    private void OnTurnOrderUnhighlight()
    {
        onTurnOrderUnhighlight();
    }

    public void SetupBattleHUD(List<BattleUnit> _turnOrder)
    {
        currentTurnOrder = _turnOrder;

        SetupTurnOrderUIItems(0);
    }

    public void SetupTurnOrderUIItems(int _currentTurn)
    {
        List<BattleUnit> uiTurnOrder = GetUITurnOrder(_currentTurn);

        for (int i = 0; i < amountOfTurnOrderUIItems; i++)
        {
            BattleUnit battleUnit = uiTurnOrder[i];
            TurnOrderUIItem currentTurnOrderUIItem = turnOrderUIItems[i];

            //setup turn order  ui items
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

    public int UpdateTurnOrderIndex(int _currentIndex)
    {
        int updatedIndex = _currentIndex + 1;

        if(updatedIndex >= currentTurnOrder.Count)
        {
            updatedIndex = 0;
        }

        return updatedIndex;
    }


    /// <summary>
    /// ///////////////////////////////////
    /// </summary>
    /// 

    private void DecrementTurnOrderIndex()
    {
        foreach (TurnOrderUIItem turnOrderUIItem in turnOrderUIItems)
        {
            //turnOrderUIItem.DecrementIndex();
        }
    }

    //Figure out doing this before rotating
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
                    //turnOrderUIItems[i].DecrementIndex();
                }
            }
        }
    }

    private IEnumerable<TurnOrderUIItem> GetAllTurnOrderUIItems()
    {
        foreach (TurnOrderUIItem turnOrderUIItem in turnOrderUIItems)
        {
            yield return turnOrderUIItem;
        }
    }

    /// <summary>
    /// ///////////////
    /// </summary>
    /// <param name="_turnOrder"></param>
    /// 
    ////////////////////////////////////Handle Deaths!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //public void RotateTurnOrder()
    //{
    //    HandleDeadUnits();
    //    DestroyOldCreateNew();
    //    turnOrderUIItems[0].SetSize(0);
    //}

    //private void DestroyOldCreateNew()
    //{
    //    TurnOrderUIItem firstTOItem = turnOrderUIItems[0];
    //    BattleUnit firstUnit = firstTOItem.GetBattleUnit();

    //    turnOrderUIItems.Remove(firstTOItem);
    //    Destroy(firstTOItem.gameObject);
    //    DecrementTurnOrderIndex();

    //    GameObject lastTOObject = Instantiate(turnOrderUIItem, turnOrderUI);
    //    TurnOrderUIItem lastTOItem = lastTOObject.GetComponent<TurnOrderUIItem>();

    //    turnOrderUIItems.Add(lastTOItem);
    //    lastTOItem.SetupTurnOrderUI(turnOrderUIItems.Count - 1, firstUnit);
    //}
}
