using System;
using UnityEngine;

namespace RPGProject.Control
{
    public class EnemyActivationTrigger : MonoBehaviour
    {
        public event Action onActivation;

        bool hasEntered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasEntered) return;
            if (other.GetComponent<PlayerController>())
            {
                hasEntered = true;
                onActivation();
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
