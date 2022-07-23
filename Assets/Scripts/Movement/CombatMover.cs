using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Movement
{
    public class CombatMover : AIMover
    {
        [SerializeField] Transform retreatTransform = null;
        [SerializeField] float distanceTolerance = .3f;

        public float gCostAllowance = 14;
        public float gCostPerAP = 24f;

        public bool isMoving = false;

        Vector3 startPosition = Vector3.zero;
        Quaternion startRotation = Quaternion.identity;

        float startStoppingDistance = 0;

        GridBlock currentBlock = null;       

        public event Action<GridBlock> onDestinationReached;

        //Refactor
        public event Action onAPSpend;

        public void InitalizeCombatMover()
        {
            isCombatMover = true;
        }

        public void SetStartVariables(GridBlock _startingBlock)
        {
            currentBlock = _startingBlock;

            startStoppingDistance = navMeshAgent.stoppingDistance;
            startPosition = transform.localPosition;
            startRotation = transform.localRotation;
        }

        public IEnumerator MoveToDestination(List<GridBlock> _path, int _furtherestBlockIndex)
        {
            if (_path == null || _path.Count <= 0) yield break;

            yield return FollowPath(_path, _furtherestBlockIndex);
        }

        private IEnumerator FollowPath(List<GridBlock> _path, int _furtherestBlockIndex)
        {
            GridBlock previousBlock = _path[0];
            GridBlock goalBlock = _path[_furtherestBlockIndex];
            isMoving = true;
            int nextBlockIndex = 1;

            while (isMoving)
            {
                GridBlock nextBlock = _path[nextBlockIndex];
                bool isGoalBlock = (goalBlock == nextBlock);

                navMeshAgent.SetDestination(GetGridBlockPosition(nextBlock));
                navMeshAgent.stoppingDistance = 0;

                bool isAtPosition = IsAtPosition(nextBlock, goalBlock);

                if (isAtPosition)
                {
                    UseMovementResources(previousBlock, nextBlock);

                    previousBlock = nextBlock;
                    if (!isGoalBlock)
                    {
                        nextBlockIndex += 1;
                        if (nextBlockIndex >= _path.Count)
                        {
                            print("The index is larger than the path count");
                            yield break;
                        }
                        yield return null;
                    }
                    else
                    {
                        onDestinationReached(goalBlock);
                        //ReachedDestination(goalBlock);
                        yield break;
                    }
                }

                yield return null;
            }
        }

        private void UseMovementResources(GridBlock _previousBlock, GridBlock _nextBlock)
        {
            float gCost = GetGCost(_previousBlock, _nextBlock);
            
            if(gCost > gCostAllowance)
            {
                gCost -= gCostAllowance;
                gCostAllowance = 0;

                onAPSpend();
                gCostAllowance = gCostPerAP;
            }

            gCostAllowance -= gCost;
        }

        private float GetGCost(GridBlock _previousBlock, GridBlock _nextBlock)
        {
            GridCoordinates previousCoords = _previousBlock.gridCoordinates;
            GridCoordinates nextCoords = _nextBlock.gridCoordinates;

            if (previousCoords.x == nextCoords.x || previousCoords.z == nextCoords.z) return 10f;
            else return 14f;
        }

        private bool IsAtPosition(GridBlock _gridBlock, GridBlock _goalBlock)
        {
            float distanceToBlock = Vector3.Distance(GetGridBlockPosition(_gridBlock), GetPlayerPosition());

            bool isEndGoal = (_gridBlock == _goalBlock);
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

        //private void ReachedDestination(GridBlock _goalBlock)
        //{
        //    currentBlock = _goalBlock;
        //    isMoving = false;
        //    isSelectingMovement = true;
        //    gridSystem.UnhighlightPath(tempPath);
        //    tempPath.Clear();
        //    path.Clear();
        //    currentIndex = 0;
        //}

        //public IEnumerator FollowPath(List<GridBlock> _path)
        //{
        //    if (_path == null || _path.Count <= 0) yield break;

        //    GridBlock goalBlock = _path[_path.Count - 1];
        //    if (goalBlock == null || goalBlock == currentBlock) yield break;

        //    yield return MoveToBlock()
        //    //gridSystem.UnhighlightPath(tempPath);
        //    //tempPath = pathfinder.FindPath(currentBlock.gridCoordinates, gridBlock.gridCoordinates);

        //    //gridSystem.HighlightPath(tempPath);

        //    //if (Input.GetMouseButtonDown(0))
        //    //{
        //    //    isSelectingMovement = false;
        //    //    path = tempPath;

        //    //    battleGridManager.GetActionPointsCost(path);
        //    //    StartCoroutine(MoveToBlock(path));
        //    //}
        //}

        public IEnumerator JumpToPos(Vector3 _jumpPosition, Quaternion _startingRotation, bool _isAttack)
        {
            if (!canMove) yield break;
            PlayAnimation(AIMovementAnimKey.Jump);

            navMeshAgent.updateRotation = false;
            UpdateStoppingDistance(!_isAttack);

            MoveTo(_jumpPosition);

            //Refactor - better calculation to determine when back at the start position
            yield return new WaitForSeconds(.75f);

            PlayAnimation(AIMovementAnimKey.Idle);

            yield return new WaitForSeconds(.5f);

            transform.localRotation = _startingRotation;

            if (navMeshAgent == null) yield break;

            UpdateStoppingDistance(false);
            navMeshAgent.updateRotation = true;
        }

        public IEnumerator Retreat()
        {
            yield return JumpToPos(retreatTransform.position, startRotation, false);
        }

        public IEnumerator ReturnToStart()
        {
            yield return JumpToPos(startPosition, startRotation, false);
        }

        public void UpdateStoppingDistance(bool _isZero)
        {
            if (_isZero)
            {
                navMeshAgent.stoppingDistance = 0;
            }
            else
            {
                navMeshAgent.stoppingDistance = startStoppingDistance;
            }
        }


        public Vector3 GetStartPosition()
        {
            return startPosition;
        }

        public Quaternion GetStartRotation()
        {
            return startRotation;
        }
    }
}
