using RPGProject.Combat;
using RPGProject.Control;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    NewBattleHandlerScript battleHandler = null;
    GridSystem gridSystem = null;
    Pathfinder pathfinder = null;

    UnitController currentUnitTurn = null;

    bool isFindingPath = false;

    List<GridBlock> tempPath = new List<GridBlock>();
    List<GridBlock> path = new List<GridBlock>();

    Ability selectedAbility = null;

    bool isRaycasting = true;

    private void Awake()
    {
        battleHandler = GetComponent<NewBattleHandlerScript>();
        gridSystem = FindObjectOfType<GridSystem>();
        pathfinder = FindObjectOfType<Pathfinder>();

        battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
    }
    private void Start()
    {
        battleHandler.GetBattleUIManager().onAbilitySelect += SetSelectedAbility;
    }

    private void SetSelectedAbility(Ability _selectedAbility)
    {
        selectedAbility = _selectedAbility;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isRaycasting = true;
        }

        if (!isRaycasting) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GridBlock goalBlock = hit.collider.GetComponentInParent<GridBlock>();
            GridBlock currentBlock = null;

            if (goalBlock != null)
            {
                if (currentUnitTurn != null) currentBlock = currentUnitTurn.currentBlock;

                if (goalBlock == currentBlock || !goalBlock.IsMovable() || currentBlock == null) return;

                gridSystem.UnhighlightPath(tempPath);
                tempPath = pathfinder.FindPath(currentBlock, goalBlock);

                int furthestBlockIndex = GetFurthestBlockIndex(tempPath, GetTotalPossibleGCostAllowance(currentUnitTurn));

                gridSystem.HighlightPath(tempPath, furthestBlockIndex);

                if (Input.GetMouseButtonDown(0))
                {
                    path = GetFurthestPath(tempPath, furthestBlockIndex);
                    OnClick();
                }
            }
            
            //if (selectedAbility != null)
            //{
            //    float attackRange = selectedAbility.attackRange;

            //    //Melee
            //    if (attackRange == 0)
            //    {
            //        //If click 
            //        ///move to target
            //        ///attack
            //    }
            //    else
            //    {
            //        // if click 
            //        ///If not in range - dont cast????
            //        ///If in range - cast
            //    }

            //    return;
            //}
            //else
            //{

            //    //Fighter fighter = hit.collider.GetComponent<Fighter>();

            //    //if (fighter != null)
            //    //{
            //    //    gridSystem.UnhighlightPath(tempPath);

            //    //    tempPath = pathfinder.FindPath(currentBlock, GetGridBlockByFighter(fighter));
            //    //}
            //}
        }     
    }

    private void OnClick()
    {
        isRaycasting = false;
        gridSystem.UnhighlightPath(tempPath);
        StartCoroutine(currentUnitTurn.PathExecution(path));
    }

    private List<GridBlock> GetFurthestPath(List<GridBlock> _path, int _furthestBlockIndex)
    {
        List<GridBlock> furthestPath = new List<GridBlock>();

        for (int i = 0; i < _path.Count; i++)
        {
            furthestPath.Add(_path[i]);

            if (i == _furthestBlockIndex) break;
        }

        return furthestPath;
    } 

    public void UpdateCurrentUnitTurn(UnitController _unitController)
    {
        currentUnitTurn = _unitController;
        if (currentUnitTurn.GetUnitInfo().IsPlayer()) isRaycasting = true;
        else isRaycasting = false;
    }

    public int GetFurthestBlockIndex(List<GridBlock> _path, float _totalGCostAllowance)
    {
        foreach (GridBlock gridBlock in _path)
        {
            if (_totalGCostAllowance >= gridBlock.pathfindingCostValues.gCost) continue;

            return _path.IndexOf(gridBlock) - 1;
        }

        return _path.Count - 1;
    }

    private float GetTotalPossibleGCostAllowance(UnitController _unitController)
    {
        float totalPossibleGCostAllowance = 0f;

        RPGProject.Movement.CombatMover combatMover = _unitController.GetMover();
        float actionPoints = _unitController.GetUnitResources().actionPoints;

        totalPossibleGCostAllowance += combatMover.gCostAllowance;
        totalPossibleGCostAllowance += combatMover.gCostPerAP * actionPoints;

        return totalPossibleGCostAllowance;
    }
}
