using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitTurnState { NoOne, Player, Enemy }

public class TurnManager : MonoBehaviour
{
    List<BattleUnit> battleUnits = new List<BattleUnit>();
    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();

    List<BattleUnit> turnOrder = new List<BattleUnit>();

    UnitTurnState unitTurnState = UnitTurnState.NoOne;
    UnitTurnState firstUnitTurnState = UnitTurnState.NoOne;

    public event Action onTurnChange;

    int turn = 0;

    public void SetUpTurns(List<BattleUnit> activeUnits, List<BattleUnit> _playerUnits, List<BattleUnit> _enemyUnits)
    {
        turn = 0;

        battleUnits = activeUnits;
        playerUnits = _playerUnits;
        enemyUnits = _enemyUnits;

        SetTurnOrder();

        unitTurnState = GetUnitTurnState();
        firstUnitTurnState = unitTurnState;
    }

    public void SetTurnOrder()
    {
        foreach (BattleUnit unitToSet in battleUnits)
        {
            turnOrder.Add(unitToSet);
        }

        turnOrder.Sort((a, b) => b.GetStats().GetSpecificStatLevel(StatType.Speed).CompareTo(a.GetStats().GetSpecificStatLevel(StatType.Speed)));
    }

    public void AdvanceTurn()
    {
        if (!GetNextBattleUnitTurn().IsDead())
        {
            if (turn + 1 < turnOrder.Count)
            {
                unitTurnState = GetNextUnitTurnState();
                turn++;
            }
            else
            {
                unitTurnState = firstUnitTurnState;
                turn = 0;
            }
        }
        else
        {
            SkipTurn();
        }

        onTurnChange();
    }
    
    public void SkipTurn()
    {
        if (turn + 1 < turnOrder.Count)
        {
            unitTurnState = GetNextUnitTurnState();
            turn++;
        }
        else
        {
            unitTurnState = firstUnitTurnState;
            turn = 0;
        }

        AdvanceTurn();
    }

    public BattleUnit GetBattleUnitTurn()
    {
        if (turn <= turnOrder.Count - 1)
        {
            return turnOrder[GetTurn()];
        }
        else
        {
            return GetFirstMoveUnit();
        }
    }

    public BattleUnit GetNextBattleUnitTurn()
    {
        if (turn <= turnOrder.Count - 2)
        {
            return turnOrder[GetTurn() + 1];
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
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if(GetBattleUnitTurn() == playerUnits[i])
            {
                unitTurnState = UnitTurnState.Player;
            }
        }

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (GetBattleUnitTurn() == enemyUnits[i])
            {
                unitTurnState = UnitTurnState.Enemy;
            }
        }

        return unitTurnState;
    }

    public UnitTurnState GetNextUnitTurnState()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (GetNextBattleUnitTurn() == playerUnits[i])
            {
                unitTurnState = UnitTurnState.Player;
            }
        }

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            if (GetNextBattleUnitTurn() == enemyUnits[i])
            {
                unitTurnState = UnitTurnState.Enemy;
            }
        }

        return unitTurnState;
    }

    public int GetTurn()
    {
        if (turn <= turnOrder.Count - 1)
        {
            return turn;
        }
        else
        {
            turn = 0;
            return turn;
        }
    }

    public bool IsUnitTurn()
    {
        if(unitTurnState == GetUnitTurnState())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPlayerTurn()
    {
        if (GetUnitTurnState() == UnitTurnState.Player)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsEnemyTurn()
    {
        if (GetUnitTurnState() == UnitTurnState.Enemy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<BattleUnit> GetTurnOrder()
    {
        return turnOrder;
    }
}
