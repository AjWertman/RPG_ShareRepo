using RPGProject.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
    public class BattleHUD : MonoBehaviour
    {
        [SerializeField] UnitResourcesIndicator unitResourcesIndicator = null;
        [SerializeField] Transform turnOrderContent = null;
        [SerializeField] GameObject turnOrderUIPrefab = null;

        TextMeshProUGUI cantCastText = null;

        int amountOfTurnOrderUIItems = 8;

        List<TurnOrderUIItem> turnOrderUIItems = new List<TurnOrderUIItem>();

        List<Fighter> uiTurnOrder = new List<Fighter>();

        public event Action<Fighter> onTurnOrderHighlight;
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

        public void UpdateTurnOrderUIItems(List<Fighter> _currentTurnOrder, Fighter _currentcombatantTurn)
        {
            uiTurnOrder = GetUITurnOrder(_currentTurnOrder, _currentcombatantTurn);
            SetupTurnOrderUIItems();
        }

        public void SetupTurnOrderUIItems()
        {
            for (int i = 0; i < amountOfTurnOrderUIItems; i++)
            {
                Fighter combatant = uiTurnOrder[i];
                TurnOrderUIItem currentTurnOrderUIItem = turnOrderUIItems[i];

                currentTurnOrderUIItem.SetupTurnOrderUI(i, combatant);
                currentTurnOrderUIItem.gameObject.SetActive(true);
            }
        }

        public void SetupUnitResourcesIndicator(Fighter _combatant)
        {
            unitResourcesIndicator.SetupResourceIndicator(_combatant);

            bool isCombatantNull = (_combatant == null);
            unitResourcesIndicator.gameObject.SetActive(!isCombatantNull);
        }

        private void OnTurnOrderHighlight(Fighter _combatant)
        {
            onTurnOrderHighlight(_combatant);
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

        public List<Fighter> GetUITurnOrder(List<Fighter> _currentTurnOrder, Fighter _currentCombatantTurn)
        {
            List<Fighter> updatedUITurnOrder = new List<Fighter>();

            int turnOrderCount = _currentTurnOrder.Count;
            int currentTurnIndex = _currentTurnOrder.IndexOf(_currentCombatantTurn);
            int index = currentTurnIndex;

            for (int i = 0; i < amountOfTurnOrderUIItems; i++)
            {
                Fighter unitTurn = _currentTurnOrder[index];
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
