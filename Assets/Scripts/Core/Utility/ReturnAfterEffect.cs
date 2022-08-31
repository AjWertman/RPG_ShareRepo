using System;
using UnityEngine;

namespace RPGProject.Core
{
    /// <summary>
    /// Plays a particle effect then returns the particle to its pool when it has completed playing.
    /// </summary>
    public class ReturnAfterEffect : MonoBehaviour
    {
        ParticleSystem myParticleSystem = null;

        public event Action<ReturnAfterEffect> onEffectCompletion;

        bool hasStarted = false;

        private void Awake()
        {
            myParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            if (myParticleSystem == null) return;
            if (myParticleSystem.isStopped)
            {
                if (!hasStarted)
                {
                    hasStarted = true;
                    onEffectCompletion(this);
                    hasStarted = false;
                }
            }
        }
    }
}