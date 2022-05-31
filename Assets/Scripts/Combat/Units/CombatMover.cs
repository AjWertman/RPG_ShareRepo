using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Combat
{
    public class CombatMover : MonoBehaviour
    {
        [SerializeField] Transform retreatTransform = null;
        [SerializeField] bool isMover = true;
        [SerializeField] bool isBattleUnit = false;

        Animator animator = null;
        NavMeshAgent navMeshAgent = null;

        Vector3 startPosition = Vector3.zero;
        Quaternion startRotation = Quaternion.identity;

        float startStoppingDistance = 0;

        public void InitalizeCombatMover()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetStartVariables()
        {
            startStoppingDistance = navMeshAgent.stoppingDistance;
            startPosition = transform.localPosition;
            startRotation = transform.localRotation;
        }

        public void MoveTo(Vector3 _destination)
        {
            if (!isMover) return;
            navMeshAgent.SetDestination(_destination);
        }

        public IEnumerator JumpToPos(Vector3 _jumpPosition, Quaternion _startingRotation, bool _isAttack)
        {

            animator.CrossFade("Jump", .1f);

            navMeshAgent.updateRotation = false;
            UpdateStoppingDistance(!_isAttack);

            //Refactor - play jump sound

            MoveTo(_jumpPosition);

            //Refactor - better calculation to determine when back at the start position
            yield return new WaitForSeconds(.75f);

            animator.CrossFade("Idle", .1f);

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

        //Refactor - Animator events create sound effects
        public void FootR()
        {
            
        }

        public void FootL()
        {
            
        }

        public void StartJump()
        {
            
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
