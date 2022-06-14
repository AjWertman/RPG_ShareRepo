using RPGProject.Movement;
using RPGProject.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPGProject.Control
{
    public class EnemyController : MonoBehaviour, ISaveable, IOverworld
    {
        [SerializeField] GameObject owMeshObject = null;
        [SerializeField] float chaseDistance = 10f;

        Animator animator = null;
        BattleZoneTrigger enemyTrigger = null;
        
        PlayerController player = null;

        AIMover mover = null;

        //Refactor - use triggers to activate level enemies?
        bool isActive = true;
        bool isBattling = false;

        bool shouldBeDisabled = false;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            mover = GetComponent<AIMover>();
            enemyTrigger = GetComponentInChildren<BattleZoneTrigger>();
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            enemyTrigger.updateShouldBeDisabled += SetShouldBeDisabled;
        }

        private void Update()
        {
            if (!isActive) return;
            if (shouldBeDisabled) return;
            if (isBattling) return;
            if (GetDistanceToPlayer() <= chaseDistance)
            {
                Chase();
            }
        }

        private void Chase()
        {
            mover.MoveTo(player.transform.position);
        }

        private float GetDistanceToPlayer()
        {
            return Vector3.Distance(transform.position, player.transform.position);
        }

        public void BattleStartBehavior()
        {
            isBattling = true;
            owMeshObject.SetActive(false);
        }

        public void BattleEndBehavior()
        {
            isBattling = false;
            if (!shouldBeDisabled)
            {
                owMeshObject.SetActive(true);
            }
            else
            {
                DisableEnemy();
            }
        }

        public void SetShouldBeDisabled(bool _shouldBeDisabled)
        {
            shouldBeDisabled = _shouldBeDisabled;
        }

        public void DisableEnemy()
        {
            owMeshObject.SetActive(false);
            enemyTrigger.gameObject.SetActive(false);

            //refactor
            GetComponent<CapsuleCollider>().enabled = false ;
            GetComponent<NavMeshAgent>().enabled = false;
            isActive = false;
        }

        public void FootR()
        {
            //CreateFootStepSound();
        }

        public void FootL()
        {
            //CreateFootStepSound();
        }

        public object CaptureState()
        {
            return shouldBeDisabled;
        }

        public void RestoreState(object _state)
        {
            bool _shouldBeDisabled = (bool)_state;
            SetShouldBeDisabled(_shouldBeDisabled);

            if (shouldBeDisabled)
            {
                DisableEnemy();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
