using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class TargetAll : AbilityBehavior
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

        public Vector3 GetCenterOfTargetsPoint(List<Fighter> _targets)
        {
            float xPos = 0f;
            float zPos = 0f;

            int targetsCount = _targets.Count;
            foreach (Fighter target in _targets)
            {
                xPos += target.transform.position.x;
                zPos += target.transform.position.z;
            }

            xPos = xPos / targetsCount;
            zPos = zPos / targetsCount;

            return new Vector3(xPos, 0, zPos);
        }
    }
}
