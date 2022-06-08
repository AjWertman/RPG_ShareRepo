using UnityEngine;

namespace RPGProject.Combat
{
    public class JustPlayEffect : AbilityBehavior
    {
        ParticleSystem abilityParticles = null;
        bool hasStarted = false;

        private void Awake()
        {
            abilityParticles = GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            if (!hasStarted) return;
            if (abilityParticles.isStopped)
            {
                hasStarted = false;
                OnAbilityDeath();
            }
        }

        public override void PerformSpellBehavior()
        {
            hasStarted = true;
        }
    }
}
