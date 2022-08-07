using RPGProject.Progression;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public enum UnitTurnState { Null, Player, Enemy }

    public class TurnManager : MonoBehaviour
    {
        List<UnitController> allUnits = new List<UnitController>();
        List<UnitController> playerUnits = new List<UnitController>();
        List<UnitController> enemyUnits = new List<UnitController>();

        List<UnitController> turnOrder = new List<UnitController>();

        UnitController currentUnitTurn = null;

        UnitTurnState unitTurnState = UnitTurnState.Null;
        UnitTurnState firstUnitTurnState = UnitTurnState.Null;

        public event Action<UnitController> onTurnChange;

        int turn = 0;

        public void SetUpTurns(List<UnitController> _activeUnits, List<UnitController> _playerUnits, List<UnitController> _enemyUnits)
        {
            allUnits = _activeUnits;
            playerUnits = _playerUnits;
            enemyUnits = _enemyUnits;

            SetTurnOrder();

            SetCurrentUnitTurn(0);
            unitTurnState = GetUnitTurnState();
            firstUnitTurnState = unitTurnState;
        }

        public void SetTurnOrder()
        {
            foreach (UnitController unitToSet in allUnits)
            {
                turnOrder.Add(unitToSet);
            }

            StatType speed = StatType.Speed;
            turnOrder.Sort((a, b) => b.GetStat(speed).CompareTo(a.GetStat(speed)));
        }

        public void UpdateTurnOrder(UnitController _unit)
        {
            bool newUnitDeathStatus = _unit.GetHealth().isDead;

            if (newUnitDeathStatus)
            {
                RemoveUnitFromTurnOrder(_unit);
            }
            else
            {
                AddUnitToTurnOrder(_unit);
            }
        }

        public void AdvanceTurn()
        {
            int currentTurnIndex = GetTurnIndex(currentUnitTurn);

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

            SetCurrentUnitTurn(currentTurnIndex);

            turn++;

            onTurnChange(currentUnitTurn);
        }

        public void SkipTurn()
        {
            int currentTurnIndex = GetTurnIndex(currentUnitTurn);
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

        public void AddUnitToTurnOrder(UnitController _unit)
        {
            if (turnOrder.Contains(_unit)) return;
            
            turnOrder.Add(_unit);

            //Refactor
            //Calculate new order position for turn order
            ///Start at end
            ///From the end to the front, use percentages to gauge if the units speed is x% larger
            ////than the next unit, move them closer to first. Increase x% each turn. 20%, 30%, 40%....
        }

        public void RemoveUnitFromTurnOrder(UnitController _unit)
        {
            if (!turnOrder.Contains(_unit)) return;

            turnOrder.Remove(_unit);
        }

        private void SetCurrentUnitTurn(int _turnIndex)
        {
            currentUnitTurn = turnOrder[_turnIndex];
        }

        public void ResetTurnManager()
        {
            allUnits.Clear();
            playerUnits.Clear();
            enemyUnits.Clear();
            turnOrder.Clear();

            currentUnitTurn = null;

            unitTurnState = UnitTurnState.Null;
            firstUnitTurnState = UnitTurnState.Null;
        }

        public UnitController GetUnitTurn()
        {
            int turnIndex = GetTurnIndex(currentUnitTurn);
            if (turnIndex <= turnOrder.Count - 1)
            {
                return turnOrder[turnIndex];
            }
            else
            {
                return GetFirstMoveUnit();
            }
        }

        public UnitController GetNextUnitTurn()
        {
            int nextTurnIndex = GetTurnIndex(currentUnitTurn) + 1;
            if (nextTurnIndex <= turnOrder.Count - 1)
            {
                return turnOrder[nextTurnIndex];
            }
            else
            {
                return GetFirstMoveUnit();
            }
        }

        public UnitController GetFirstMoveUnit()
        {
            return turnOrder[0];
        }

        public UnitTurnState GetUnitTurnState()
        {
            bool isPlayer = GetUnitTurn().GetUnitInfo().isPlayer;

            if (isPlayer) return UnitTurnState.Player;
            else return UnitTurnState.Enemy;
        }

        public UnitTurnState GetNextUnitTurnState()
        {
            bool isPlayer = GetNextUnitTurn().GetUnitInfo().isPlayer;

            if (isPlayer) return UnitTurnState.Player;
            else return UnitTurnState.Enemy;
        }

        public List<UnitController> GetTurnOrder()
        {
            return turnOrder;
        }

        public UnitController GetCurrentUnitTurn()
        {
            return currentUnitTurn;
        }

        private int GetTurnIndex(UnitController _unit)
        {
            return turnOrder.IndexOf(_unit);
        }

        public bool IsPlayerTurn()
        {
            return (GetUnitTurnState() == UnitTurnState.Player);
        }
    }
}