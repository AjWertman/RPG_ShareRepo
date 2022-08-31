using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Behavior that only plays its particle FX and does damage/healing or applies buff/debuff then dies.
    /// </summary>
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

        public override void PerformAbilityBehavior()
        {
            hasStarted = true;
        }
    }
}
