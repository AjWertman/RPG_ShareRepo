﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Movement
{
    /// <summary>
    /// Handles the movement for combatants.
    /// </summary>
    public class CombatMover : AIMover
    {
        [SerializeField] float distanceTolerance = .3f;
            
        [SerializeField] float baseMovementSpeed = 6f;
        [SerializeField] float modifierPercentage = .2f;
        [SerializeField] float maxMovementSpeed = 15f;

        public float speed = 0f;

        public bool isMoving = false;

        float startStoppingDistance = 0;

        IEnumerator currentPath = null;

        public event Action onBlockReached;

        public void InitalizeCombatMover()
        {
            isCombatMover = true;
        }

        public void SetSpeed(float _speed)
        {
            speed = _speed;
            navMeshAgent.speed = CalculateMoveSpeed(speed);
        }

        public IEnumerator MoveToDestination(List<Transform> _path)
        {
            if (_path == null || _path.Count <= 0) yield break;

            currentPath = FollowPath(_path);

            yield return currentPath;
        }

        private IEnumerator FollowPath(List<Transform> _path)
        {
            Transform previousTransform = _path[0];
            Transform goalTransform = _path[_path.Count -1];
            isMoving = true;
            UpdateAnimator(true);
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
                    onBlockReached();

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
                        UpdateAnimator(false);
                        isMoving = false;
                        yield break;
                    }
                }

                yield return null;
            }
        }
        
        public void Teleport(Vector3 _teleportPosition)
        {
            StopCoroutine(currentPath);

            navMeshAgent.enabled = false;
            transform.position = _teleportPosition;
            navMeshAgent.enabled = true;
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
        
        public void SetAnimator(Animator _animator)
        {
            animator = _animator;
        }

        private void UpdateAnimator(bool _isMoving)
        {
            animator.SetBool("isMoving", _isMoving);
        }

        private float CalculateMoveSpeed(float _speed)
        {
            float speedModifier = _speed - 10;
            if (speedModifier <= 0) return baseMovementSpeed;

            float speedPerModPoint = baseMovementSpeed * modifierPercentage;

            float totalSpeed = baseMovementSpeed;
            for (int i = 0; i < speedModifier; i++)
            {
                totalSpeed += speedPerModPoint;
            }
            totalSpeed = Mathf.Clamp(totalSpeed, baseMovementSpeed, maxMovementSpeed);
            return totalSpeed;
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

        private int GetGCost(Transform _previousTransform, Transform _nextTransform)
        {
            if (_previousTransform.localPosition.x == _nextTransform.localPosition.x || _previousTransform.localPosition.z == _nextTransform.localPosition.z) return 10;
            else return 14;
        }      
    }
}
