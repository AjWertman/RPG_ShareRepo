using RPGProject.Combat;
using RPGProject.Control;
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

    private void Awake()
    {
        battleHandler = GetComponent<NewBattleHandlerScript>();
        gridSystem = FindObjectOfType<GridSystem>();
        pathfinder = FindObjectOfType<Pathfinder>();

        battleHandler.onUnitTurnUpdate += UpdateCurrentUnitTurn;
    }

    void Update()
    {
        //Refactor - Need a check to ask if player is moving. If so, dont look for paths
        if (isFindingPath)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GridBlock goalBlock = hit.collider.GetComponentInParent<GridBlock>();
                GridBlock currentBlock = currentUnitTurn.currentBlock;

                if (goalBlock != null)
                {
                    if (goalBlock == currentBlock || !goalBlock.IsMovable()) return;

                    gridSystem.UnhighlightPath(tempPath);
                    tempPath = pathfinder.FindPath(currentBlock, goalBlock);

                    int furtherestBlockIndex = GetFurtherestBlockIndex(tempPath, GetTotalPossibleGCostAllowance(currentUnitTurn));

                    gridSystem.HighlightPath(tempPath, furtherestBlockIndex);

                    if (Input.GetMouseButtonDown(0))
                    {
                        //isFindingPath = false;
                        StartCoroutine(currentUnitTurn.GetMover().MoveToDestination(tempPath, furtherestBlockIndex));
                        gridSystem.UnhighlightPath(tempPath);
                    }
                }

                //Fighter fighter = hit.collider.GetComponent<Fighter>();

                //if (fighter != null)
                //{
                //    gridSystem.UnhighlightPath(tempPath);

                //    tempPath = pathfinder.FindPath(currentBlock, GetGridBlockByFighter(fighter));
                //}
            }
        }
    }

    public void UpdateCurrentUnitTurn(UnitController _unitController)
    {
        currentUnitTurn = _unitController;
        if (currentUnitTurn.GetUnitInfo().IsPlayer()) isFindingPath = true;
        else isFindingPath = false;
    }

    public int GetFurtherestBlockIndex(List<GridBlock> _path, float _totalGCostAllowance)
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
