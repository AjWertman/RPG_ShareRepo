using RPGProject.Control.Combat;
using RPGProject.Movement;
using RPGProject.Saving;
using RPGProject.Sound;
using UnityEngine;

namespace RPGProject.Control
{
    public class EnemyController : MonoBehaviour, ISaveable, IOverworld
    {
        [SerializeField] GameObject owMeshObject = null;
        [SerializeField] float chaseDistance = 10f;

        Animator animator = null;
        BattleZoneTrigger enemyTrigger = null;
        CapsuleCollider capsuleCollider = null;
        
        PlayerController player = null;
        SoundFXManager soundFXManager = null;

        AIMover mover = null;

        bool isActive = true;
        bool isBattling = false;

        bool shouldBeDisabled = false;

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            mover = GetComponent<AIMover>();
            enemyTrigger = GetComponentInChildren<BattleZoneTrigger>();
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerController>();
            soundFXManager = FindObjectOfType<SoundFXManager>();
            enemyTrigger.updateShouldBeDisabled += SetShouldBeDisabled;

            DeactivateEnemy();
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
                DeactivateEnemy();
            }
        }

        public void SetShouldBeDisabled(bool _shouldBeDisabled)
        {
            shouldBeDisabled = _shouldBeDisabled;
        }

        public void ActivateEnemy()
        {
            owMeshObject.SetActive(true);
            enemyTrigger.gameObject.SetActive(true);
            capsuleCollider.enabled = true;
            mover.enabled = true;
            isActive = true;
        }

        public void DeactivateEnemy()
        {
            owMeshObject.SetActive(false);
            enemyTrigger.gameObject.SetActive(false);
            capsuleCollider.enabled = false;
            mover.enabled = false;
            isActive = false;
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
                DeactivateEnemy();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
