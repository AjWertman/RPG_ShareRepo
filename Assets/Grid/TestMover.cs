using RPGProject.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestMover : MonoBehaviour
{
    [SerializeField] float distanceTolerance = .3f;

    List<GridBlock> path = new List<GridBlock>();
    List<GridBlock> tempPath = new List<GridBlock>();

    GridSystem gridSystem = null;
    Pathfinder pathfinder = null;
    NavMeshAgent navMeshAgent = null;

    BattleGridManager battleGridManager = null;

    GridBlock currentBlock = null;
    int currentIndex = 0;

    bool isSelectingMovement = true;
    bool isMoving = false;

    private void Awake()
    {
        gridSystem = FindObjectOfType<GridSystem>();
        pathfinder = FindObjectOfType<Pathfinder>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        battleGridManager = FindObjectOfType<BattleGridManager>();
    }

    private void Start()
    {
        currentBlock = pathfinder.GetGridBlock(0, 0);
    }

    private void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        //if (!isSelectingMovement) return;

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit))
        //{
        //    GridBlock gridBlock = hit.collider.GetComponentInParent<GridBlock>();

        //    if (gridBlock == null) return;
        //    if (gridBlock == currentBlock) return;
        //    gridSystem.UnhighlightPath(tempPath);
        //    tempPath = pathfinder.FindPath(currentBlock, gridBlock);

        //    //gridSystem.HighlightPath(tempPath);

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        isSelectingMovement = false;
        //        path = tempPath;

        //        battleGridManager.GetActionPointsCost(path);
        //        StartCoroutine(MoveToBlock(path));
        //    }
        //}
    }

    private IEnumerator MoveToBlock(List<GridBlock> _path)
    {
        yield return FollowPath(_path);
    }

    private IEnumerator FollowPath(List<GridBlock> _path)
    {
        GridBlock goalBlock = _path[_path.Count - 1];
        isMoving = true;

        currentIndex = 1;
        while (isMoving)
        {
            GridBlock nextBlock = _path[currentIndex];
            bool isGoalBlock = goalBlock == nextBlock;

            navMeshAgent.SetDestination(GetGridBlockPosition(nextBlock));         

            bool isAtPosition = IsAtPosition(nextBlock);

            if (isAtPosition)
            {
                if (!isGoalBlock)
                {                   
                    currentIndex += 1;
                    if (currentIndex >= path.Count)
                    {
                        print("The index is larger than the path count");
                        yield break;
                    }
                    yield return null;
                }
                else
                {
                    ReachedDestination(goalBlock);
                    yield break;
                }
            }

            yield return null;
        }
    }

    private void ReachedDestination(GridBlock _goalBlock)
    {
        currentBlock = _goalBlock;
        isMoving = false;
        isSelectingMovement = true;
        gridSystem.UnhighlightPath(tempPath);
        tempPath.Clear();
        path.Clear();
        currentIndex = 0;
    }

    private bool IsAtPosition(GridBlock _gridBlock)
    {
        float distanceToBlock = Vector3.Distance(GetGridBlockPosition(_gridBlock), GetPlayerPosition());

        bool isEndGoal = (_gridBlock == path[path.Count - 1]);
        if (isEndGoal)
        {
            return distanceToBlock == 0;
        }
        
        return distanceToBlock < distanceTolerance;
    }

    private Vector3 GetPlayerPosition()
    {
        return new Vector3(transform.position.x, 0, transform.position.z);
    }

    private Vector3 GetGridBlockPosition(GridBlock _gridBlock)
    {
        Vector3 goalDestination = _gridBlock.travelDestination.position;

        return new Vector3(goalDestination.x, 0, goalDestination.z);
    }
}