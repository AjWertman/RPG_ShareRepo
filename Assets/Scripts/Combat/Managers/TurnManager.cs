using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public enum UnitTurnState { Null, Player, Enemy }

    public class TurnManager : MonoBehaviour
    {
        List<BattleUnit> battleUnits = new List<BattleUnit>();
        List<BattleUnit> playerUnits = new List<BattleUnit>();
        List<BattleUnit> enemyUnits = new List<BattleUnit>();

        List<BattleUnit> turnOrder = new List<BattleUnit>();

        BattleUnit currentBattleUnitTurn = null;

        UnitTurnState unitTurnState = UnitTurnState.Null;
        UnitTurnState firstUnitTurnState = UnitTurnState.Null;

        public event Action<BattleUnit> onTurnChange;

        int turn = 0;

        public void SetUpTurns(List<BattleUnit> _activeUnits, List<BattleUnit> _playerUnits, List<BattleUnit> _enemyUnits)
        {
            battleUnits = _activeUnits;
            playerUnits = _playerUnits;
            enemyUnits = _enemyUnits;

            SetTurnOrder();

            SetCurrentBattleUnitTurn(0);
            unitTurnState = GetUnitTurnState();
            firstUnitTurnState = unitTurnState;
        }

        public void SetTurnOrder()
        {
            foreach (BattleUnit unitToSet in battleUnits)
            {
                turnOrder.Add(unitToSet);
            }

            turnOrder.Sort((a, b) => b.GetStat(StatType.Speed).GetStatLevel().CompareTo(a.GetStat(StatType.Speed).GetStatLevel()));
        }

        public void UpdateTurnOrder(BattleUnit _battleUnit)
        {
            bool newUnitDeathStatus = _battleUnit.GetHealth().IsDead();

            if (newUnitDeathStatus)
            {
                RemoveUnitFromTurnOrder(_battleUnit);
            }
            else
            {
                AddUnitToTurnOrder(_battleUnit);
            }
        }

        public void AdvanceTurn()
        {
            int currentTurnIndex = GetTurnIndex(currentBattleUnitTurn);

            if (currentTurnIndex + 1 < turnOrder.Count)
            {
                unitTurnState = GetNextUnitTurnState();
                currentTurnIndex++;
            }
            else
            {
                unitTurnState = firstUnitTurnState;
                currentTurnIndex = 0;
            }

            SetCurrentBattleUnitTurn(currentTurnIndex);

            turn++;

            onTurnChange(currentBattleUnitTurn);
        }

        public void SkipTurn()
        {
            int currentTurnIndex = GetTurnIndex(currentBattleUnitTurn);
            if (currentTurnIndex + 1 < turnOrder.Count)
            {
                unitTurnState = GetNextUnitTurnState();
                currentTurnIndex++;
            }
            else
            {
                unitTurnState = firstUnitTurnState;
                currentTurnIndex = 0;
            }

            AdvanceTurn();
        }

        public void AddUnitToTurnOrder(BattleUnit _battleUnit)
        {
            if (turnOrder.Contains(_battleUnit)) return;
            
            turnOrder.Add(_battleUnit);

            //Refactor
            //Calculate new order position for turn order
            ///Start at end
            ///From the end to the front, use percentages to gauge if the units speed is x% larger
            ////than the next unit, move them closer to first. Increase x% each turn. 20%, 30%, 40%....
        }

        public void RemoveUnitFromTurnOrder(BattleUnit _battleUnit)
        {
            if (!turnOrder.Contains(_battleUnit)) return;

            turnOrder.Remove(_battleUnit);
        }

        private void SetCurrentBattleUnitTurn(int _turnIndex)
        {
            currentBattleUnitTurn = turnOrder[_turnIndex];
        }

        public void ResetTurnManager()
        {
            battleUnits.Clear();
            playerUnits.Clear();
            enemyUnits.Clear();
            turnOrder.Clear();

            currentBattleUnitTurn = null;

            unitTurnState = UnitTurnState.Null;
            firstUnitTurnState = UnitTurnState.Null;
        }

        public BattleUnit GetBattleUnitTurn()
        {
            int turnIndex = GetTurnIndex(currentBattleUnitTurn);
            if (turnIndex <= turnOrder.Count - 1)
            {
                return turnOrder[turnIndex];
            }
            else
            {
                return GetFirstMoveUnit();
            }
        }

        public BattleUnit GetNextBattleUnitTurn()
        {
            int nextTurnIndex = GetTurnIndex(currentBattleUnitTurn) + 1;
            if (nextTurnIndex <= turnOrder.Count - 1)
            {
                return turnOrder[nextTurnIndex];
            }
            else
            {
                return GetFirstMoveUnit();
            }
        }

        public BattleUnit GetFirstMoveUnit()
        {
            return turnOrder[0];
        }

        public UnitTurnState GetUnitTurnState()
        {
            bool isPlayer = GetBattleUnitTurn().GetBattleUnitInfo().IsPlayer();

            if (isPlayer) return UnitTurnState.Player;
            else return UnitTurnState.Enemy;
        }

        public UnitTurnState GetNextUnitTurnState()
        {
            bool isPlayer = GetNextBattleUnitTurn().GetBattleUnitInfo().IsPlayer();

            if (isPlayer) return UnitTurnState.Player;
            else return UnitTurnState.Enemy;
        }

        public List<BattleUnit> GetTurnOrder()
        {
            return turnOrder;
        }

        public BattleUnit GetCurrentBattleUnitTurn()
        {
            return currentBattleUnitTurn;
        }

        private int GetTurnIndex(BattleUnit _battleUnit)
        {
            return turnOrder.IndexOf(_battleUnit);
        }

        public bool IsPlayerTurn()
        {
            return (GetUnitTurnState() == UnitTurnState.Player);
        }
    }
}