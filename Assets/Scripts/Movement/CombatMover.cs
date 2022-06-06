using System.Collections;
using UnityEngine;

namespace RPGProject.Movement
{
    public class CombatMover : AIMover
    {
        [SerializeField] Transform retreatTransform = null;

        Vector3 startPosition = Vector3.zero;
        Quaternion startRotation = Quaternion.identity;

        float startStoppingDistance = 0;

        public void InitalizeCombatMover()
        {
            isCombatMover = true;
        }

        public void SetStartVariables()
        {
            startStoppingDistance = navMeshAgent.stoppingDistance;
            startPosition = transform.localPosition;
            startRotation = transform.localRotation;
        }

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
