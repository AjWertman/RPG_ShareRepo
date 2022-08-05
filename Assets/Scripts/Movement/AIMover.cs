using RPGProject.Sound;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Movement
{
    public enum AIMovementAnimKey { Idle, Jump , Moving}

    public abstract class AIMover : MonoBehaviour
    {
        [SerializeField] protected AudioClip footstepsSound = null;
        protected Animator animator = null;
        protected NavMeshAgent navMeshAgent = null;
        protected SoundFXManager soundFXManager = null;

        protected bool canMove = true;
        protected bool isCombatMover = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            soundFXManager = FindObjectOfType<SoundFXManager>();
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

        private void CreateFootstepsSound()
        {
            soundFXManager.CreateSoundFX(footstepsSound, transform, .75f);
        }

        public void FootR()
        {
            CreateFootstepsSound();
        }

        public void FootL()
        {
            CreateFootstepsSound();
        }

        public void StartJump()
        {
            CreateFootstepsSound();
        }
    }
}