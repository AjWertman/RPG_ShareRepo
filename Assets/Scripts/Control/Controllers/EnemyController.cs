using RPGProject.Combat;
using RPGProject.Saving;
using UnityEngine;

namespace RPGProject.Control
{
    public class EnemyController : MonoBehaviour, ISaveable, IOverworld
    {
        [SerializeField] GameObject owMeshObject = null;
        [SerializeField] float chaseDistance = 10f;

        Animator animator = null;
        BattleZoneTrigger enemyTrigger = null;
        

        PlayerController player = null;

        //Refactor - use triggers to activate level enemies?
        bool isActive = false;
        bool isBattling = false;

        bool shouldBeDisabled = false;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            //mover = GetComponent<CombatMover>();
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
            //mover.MoveTo(player.transform.position);
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
