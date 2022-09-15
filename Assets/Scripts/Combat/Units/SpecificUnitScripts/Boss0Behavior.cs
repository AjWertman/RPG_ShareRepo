using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class Boss0Behavior : UniqueUnitBehavior
    {
        [SerializeField] Ability explosionAbility = null;
        [SerializeField] float percentageOfHealthUntilExplosion = .25f;

        float damageTaken = 0f;
        float damageUntilExplosion = 0f;

        bool hasExplosionQueued = false;

        public override void Initialize()
        {
            base.Initialize();

            health.onHealthChange += RecordDamageTaken;
            fighter.onAbilityUse += ResetExplosionBehavior;       
            damageUntilExplosion = health.maxHealthPoints * percentageOfHealthUntilExplosion;
        }

        private void RecordDamageTaken(bool NaN, float _damage)
        {
            if (hasExplosionQueued) return;
            if (_damage > 0) return;
            float damage = Mathf.Abs(_damage);

            damageTaken += damage;

            if(damageTaken > damageUntilExplosion)
            {
                fighter.selectedAbility = explosionAbility;
                damageTaken = 0;
                hasExplosionQueued = true;
            }
        }

        private void ResetExplosionBehavior()
        {
            if (!hasExplosionQueued) return;
            hasExplosionQueued = false;
        }

        public override List<AbilityBehavior> GetAbilityBehaviors()
        {
            List<AbilityBehavior> abilityBehaviors = new List<AbilityBehavior>();
            foreach(ComboLink comboLink in explosionAbility.combo)
            {
                abilityBehaviors.Add(comboLink.abilityBehavior);
            }
            return abilityBehaviors;
        }
    }
}