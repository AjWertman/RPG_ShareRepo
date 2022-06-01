using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Movement
{
    public enum AIMovementAnimKey { Idle, Jump }

    public abstract class AIMover : MonoBehaviour
    {
        protected Animator animator = null;
        protected NavMeshAgent navMeshAgent = null;

        protected bool canMove = true;
        protected bool isCombatMover = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 _destination)
        {
            if (!canMove) return;
            navMeshAgent.SetDestination(_destination);
        }

        protected void PlayAnimation(AIMovementAnimKey _aiMovementAnimKey)
        {
            animator.CrossFade(_aiMovementAnimKey.ToString(), .1f);
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
    }
}