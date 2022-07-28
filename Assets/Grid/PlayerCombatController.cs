using RPGProject.Combat;
using RPGProject.Control;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    BattleGridManager battleGridManager = null;
    NewBattleHandlerScript battleHandler = null;
    GridSystem gridSystem = null;
    Pathfinder pathfinder = null;

    UnitController currentUnitTurn = null;

    bool isFindingPath = false;

    List<GridBlock> tempPath = new List<GridBlock>();
    List<GridBlock> path = new List<GridBlock>();
    int furthestBlockIndex = 0;

    Ability selectedAbility = null;

    bool isRaycasting = false;

    private void Awake()
    {
        battleGridManager = FindObjectOfType<BattleGridManager>();
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
            CombatTarget combatTarget = hit.collider.GetComponent<CombatTarget>();

            if (combatTarget != null)
            {
                GridBlock targetBlock = GetTargetBlock(combatTarget);
                HandlePathfinding(targetBlock);

                if (Input.GetMouseButtonDown(0))
                {
                    if (tempPath == null) return;
                    path = GetFurthestPath(tempPath, furthestBlockIndex);
                    OnClick(targetBlock);
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

    private GridBlock GetTargetBlock(CombatTarget _combatTarget)
    {
        GridBlock targetBlock = null;
        Fighter fighter = _combatTarget.GetComponent<Fighter>();
        if (fighter != null)
        {
            targetBlock = battleGridManager.GetGridBlockByFighter(fighter);
        }
        else
        {
            targetBlock = _combatTarget.GetComponent<GridBlock>();            
        }
        return targetBlock;
    }

    private void HandlePathfinding(GridBlock _targetBlock)
    {
        if (_targetBlock == null) return;

        GridBlock currentBlock = null;
        if (currentUnitTurn != null) currentBlock = currentUnitTurn.currentBlock;

        if (_targetBlock == currentBlock || !_targetBlock.IsMovable() || currentBlock == null)
        {
            return;
        }

        gridSystem.UnhighlightPath(tempPath);
        tempPath = pathfinder.FindPath(currentBlock, _targetBlock);

        furthestBlockIndex = GetFurthestBlockIndex(tempPath, GetTotalPossibleGCostAllowance(currentUnitTurn));

        gridSystem.HighlightPath(tempPath, furthestBlockIndex);
    }

    private void OnClick(GridBlock _targetBlock)
    {
        isRaycasting = false;
        gridSystem.UnhighlightPath(tempPath);
        
        if(selectedAbility != null)
        {
            CombatTarget target = _targetBlock;
            Fighter targetBlockFighter = _targetBlock.contestedFighter;

            if(targetBlockFighter != null)
            {
                target = targetBlockFighter;
            }
             
            if (target == null) return;

            if (selectedAbility.requiresTarget && targetBlockFighter == null) return;

            battleHandler.OnPlayerMove(target, selectedAbility);
            selectedAbility = null;
        }
        else
        {
            StartCoroutine(currentUnitTurn.PathExecution(path));
        }
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
