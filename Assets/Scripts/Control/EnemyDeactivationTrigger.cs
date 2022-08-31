using System;
using UnityEngine;

namespace RPGProject.Control
{    /// <summary>
     /// Trigger that deactivates overworld enemies for specific areas.
     /// </summary>
    public class EnemyDeactivationTrigger : MonoBehaviour
    {
        public event Action onDeactivation;

        bool hasEntered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasEntered) return;
            if (other.GetComponent<PlayerController>())
            {
                hasEntered = true;
                onDeactivation();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            hasEntered = false;
        }
        
        public void DeactivateTrigger()
        {
            hasEntered = false;
            gameObject.SetActive(false);
        }
    }
}
