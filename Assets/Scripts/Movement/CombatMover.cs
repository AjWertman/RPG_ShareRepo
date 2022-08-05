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

        public event Action onAPSpend;

        public void InitalizeCombatMover()
        {
            isCombatMover = true;
        }

        public IEnumerator MoveToDestination(List<Transform> _path)
        {
            if (_path == null || _path.Count <= 0) yield break;

            yield return FollowPath(_path);
        }

        private IEnumerator FollowPath(List<Transform> _path)
        {
            Transform previousTransform = _path[0];
            Transform goalTransform = _path[_path.Count -1];
            isMoving = true;
            int nextBlockIndex = 1;

            navMeshAgent.stoppingDistance = 0;

            while (isMoving)
            {
                Transform nextTransform = _path[nextBlockIndex];
                bool isGoalBlock = (goalTransform == nextTransform);

                navMeshAgent.SetDestination(GetOffsetPosition(nextTransform));

                bool isAtPosition = IsAtPosition(nextTransform, goalTransform);

                if (isAtPosition)
                {
                    UseMovementResources(previousTransform, nextTransform);

                    previousTransform = nextTransform;
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
                        yield break;
                    }
                }

                yield return null;
            }
        }

        private void UseMovementResources(Transform _previousBlock, Transform _nextBlock)
        {
            float gCost = GetGCost(_previousBlock, _nextBlock);

            if (gCost > gCostAllowance)
            {
                gCost -= gCostAllowance;
                gCostAllowance = 0;

                onAPSpend();
                gCostAllowance = gCostPerAP;
            }

            gCostAllowance -= gCost;
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

        private bool IsAtPosition(Transform _gridBlock, Transform _goalBlock)
        {
            float distanceToBlock = Vector3.Distance(GetOffsetPosition(_gridBlock), GetOffsetPosition(transform));

            bool isEndGoal = (_gridBlock == _goalBlock);
            if (isEndGoal)
            {
                return distanceToBlock == 0;
            }

            return distanceToBlock < distanceTolerance;
        }

        private Vector3 GetOffsetPosition(Transform _transform)
        {
            return new Vector3(_transform.position.x, 0, _transform.position.z);
        }

        public Vector3 GetStartPosition()
        {
            return startPosition;
        }

        public Quaternion GetStartRotation()
        {
            return startRotation;
        }

        private float GetGCost(Transform _previousTransform, Transform _nextTransform)
        {
            if (_previousTransform.localPosition.x == _nextTransform.localPosition.x || _previousTransform.localPosition.z == _nextTransform.localPosition.z) return 10f;
            else return 14f;
        }      
    }
}
