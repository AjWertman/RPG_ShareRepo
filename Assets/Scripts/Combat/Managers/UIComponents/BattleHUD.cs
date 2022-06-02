using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPGProject.Combat
{
    public class BattleHUD : MonoBehaviour
    {
        [SerializeField] UnitResourcesIndicator unitResourcesIndicator = null;
        [SerializeField] Transform turnOrderContent = null;
        [SerializeField] GameObject turnOrderUIPrefab = null;

        TextMeshProUGUI cantCastText = null;

        int amountOfTurnOrderUIItems = 8;

        List<TurnOrderUIItem> turnOrderUIItems = new List<TurnOrderUIItem>();

        List<BattleUnit> uiTurnOrder = new List<BattleUnit>();

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
                turnOrderUIItem.InitalizeTurnOrderUIItem();

                turnOrderUIItem.onPointerEnter += OnTurnOrderHighlight;
                turnOrderUIItem.onPointerExit += OnTurnOrderUnhighlight;

                turnOrderUIItems.Add(turnOrderUIItem);

                turnOrderInstance.gameObject.SetActive(false);
            }
        }

        public void UpdateTurnOrderUIItems(List<BattleUnit> _currentTurnOrder, BattleUnit _currentBattleUnitTurn)
        {
            uiTurnOrder = GetUITurnOrder(_currentTurnOrder, _currentBattleUnitTurn);
            SetupTurnOrderUIItems();
        }

        public void SetupTurnOrderUIItems()
        {
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

        public List<BattleUnit> GetUITurnOrder(List<BattleUnit> _currentTurnOrder, BattleUnit _currentBattleUnitTurn)
        {
            List<BattleUnit> updatedUITurnOrder = new List<BattleUnit>();

            int turnOrderCount = _currentTurnOrder.Count;
            int currentTurnIndex = _currentTurnOrder.IndexOf(_currentBattleUnitTurn);
            int index = currentTurnIndex;

            for (int i = 0; i < amountOfTurnOrderUIItems; i++)
            {
                BattleUnit unitTurn = _currentTurnOrder[index];
                updatedUITurnOrder.Add(unitTurn);

                index = UpdateIndex(index, turnOrderCount);
            }

            return updatedUITurnOrder;
        }

        private int UpdateIndex(int _index, int _turnOrderCount)
        {
            int updatedIndex = _index + 1;

            if (updatedIndex >= _turnOrderCount)
            {
                updatedIndex = 0;
            }

            return updatedIndex;
        }

        public IEnumerator ActivateCantUseAbilityUI(string _reason)
        {
            yield return CantUseAbilityUIBehavior(_reason);
        }

        public IEnumerator CantUseAbilityUIBehavior(string _reason)
        {
            cantCastText.text = _reason;
            cantCastText.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            cantCastText.text = "";
            cantCastText.gameObject.SetActive(false);
        }
    }
}
