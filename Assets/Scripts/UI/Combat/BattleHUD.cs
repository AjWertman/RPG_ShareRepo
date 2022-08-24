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
        [SerializeField] SelectedUnitIndicator selectedUnitIndicator = null;

        [SerializeField] Transform turnOrderContent = null;
        [SerializeField] TurnOrderUIItem turnOrderUIPrefab = null;

        [SerializeField] Transform teamMemberContent = null;
        [SerializeField] TeamMemberIndicator teamMemberIndicatorPrefab = null;

        [SerializeField] TextMeshProUGUI cantCastText = null;

        int amountOfTurnOrderUIItems = 10;

        List<TurnOrderUIItem> turnOrderUIItems = new List<TurnOrderUIItem>();
        List<TeamMemberIndicator> teamMemberIndicators = new List<TeamMemberIndicator>();

        List<Fighter> uiTurnOrder = new List<Fighter>();

        public event Action<Fighter, bool> onFighterHighlight;
        public event Action onFighterUnhighlight;

        private void Awake()
        {
            CreateTurnOrderUIItemPool();
            CreateTeamMemberUIItemPool();
            selectedUnitIndicator.DeactivateIndicator();

            cantCastText.text = "";
            cantCastText.gameObject.SetActive(false);
        }

        private void CreateTurnOrderUIItemPool()
        {
            foreach (Transform existingUIItem in turnOrderContent)
            {
                Destroy(existingUIItem.gameObject);
            }

            for (int i = 0; i < amountOfTurnOrderUIItems; i++)
            {
                TurnOrderUIItem turnOrderInstance = Instantiate(turnOrderUIPrefab, turnOrderContent);
                turnOrderInstance.InitalizeTurnOrderUIItem();

                turnOrderInstance.onHighlight += OnFighterHighlight;
                turnOrderInstance.onUnhighlight += OnFighterUnhighlight;

                turnOrderUIItems.Add(turnOrderInstance);

                turnOrderInstance.gameObject.SetActive(false);
            }
        }

        private void CreateTeamMemberUIItemPool()
        {
            foreach(Transform existingIndicator in teamMemberContent)
            {
                Destroy(existingIndicator.gameObject);
            }

            for (int i = 0; i < 4; i++)
            {
                TeamMemberIndicator teamMemberIndicator = Instantiate(teamMemberIndicatorPrefab, teamMemberContent);
                teamMemberIndicator.onHighlight += OnFighterHighlight;
                teamMemberIndicator.onUnhighlight += OnFighterUnhighlight;
                    
                teamMemberIndicators.Add(teamMemberIndicator);
               
                teamMemberIndicator.gameObject.SetActive(false);
            }
        }

        public void UpdateTurnOrderUIItems(List<Fighter> _currentTurnOrder, Fighter _currentcombatantTurn)
        {
            uiTurnOrder = GetUITurnOrder(_currentTurnOrder, _currentcombatantTurn);
            SetupTurnOrderUIItems();
        }

        public void SetupTeammemberIndicators(List<Fighter> _teamMembers)
        {
            for (int i = 0; i < _teamMembers.Count; i++)
            {
                teamMemberIndicators[i].SetupIndicator(_teamMembers[i]);
                teamMemberIndicators[i].gameObject.SetActive(true);
            }
        }

        public void SetupTurnOrderUIItems()
        {
            for (int i = 0; i < amountOfTurnOrderUIItems; i++)
            {
                Fighter combatant = uiTurnOrder[i];
                TurnOrderUIItem currentTurnOrderUIItem = turnOrderUIItems[i];

                currentTurnOrderUIItem.SetupTurnOrderUI(combatant);
                currentTurnOrderUIItem.gameObject.SetActive(true);
            }
        }

        public void SetupUnitResourcesIndicator(Fighter _combatant)
        {
            if (_combatant.unitInfo.isPlayer)
            {
                selectedUnitIndicator.OnTeammateSelection(GetTeamMemberIndicator(_combatant));
            }
            else
            {
                selectedUnitIndicator.SetupResourceIndicator(_combatant);
            }

            selectedUnitIndicator.gameObject.SetActive(true);
        }

        public void IssueCheck()
        {

        }

        public TeamMemberIndicator GetTeamMemberIndicator(Fighter _fighter)
        {
            foreach (TeamMemberIndicator teamMemberIndicator in teamMemberIndicators)
            {
                if (teamMemberIndicator.GetFighter() == _fighter) return teamMemberIndicator;
            }

            return null;
        }

        private void OnFighterHighlight(Fighter _combatant)
        {
            onFighterHighlight(_combatant, true);
        }

        private void OnFighterUnhighlight()
        {
            onFighterUnhighlight();
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

        public SelectedUnitIndicator GetSelectedTargetIndicator()
        {
            return selectedUnitIndicator;
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

        private IEnumerator CantUseAbilityUIBehavior(string _reason)
        {
            cantCastText.text = _reason;
            cantCastText.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.5f);

            cantCastText.text = "";
            cantCastText.gameObject.SetActive(false);
        }
    }
}
