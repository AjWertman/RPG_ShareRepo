using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Movement
{
    public class CombatMover : AIMover
    {
        [SerializeField] float distanceTolerance = .3f;

        public float gCostPerAP = 24f;
        public bool isMoving = false;

        float startStoppingDistance = 0;

        public event Action<float> onBlockReached;

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

                if (nextTransform == null) yield break;

                bool isGoalBlock = (goalTransform == nextTransform);

                navMeshAgent.SetDestination(GetOffsetPosition(nextTransform));

                bool isAtPosition = IsAtPosition(nextTransform, goalTransform);

                if (isAtPosition)
                {
                    float gCost = GetGCost(previousTransform, nextTransform);
                    onBlockReached(gCost);

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

        private float GetGCost(Transform _previousTransform, Transform _nextTransform)
        {
            if (_previousTransform.localPosition.x == _nextTransform.localPosition.x || _previousTransform.localPosition.z == _nextTransform.localPosition.z) return 10f;
            else return 14f;
        }      
    }
}
