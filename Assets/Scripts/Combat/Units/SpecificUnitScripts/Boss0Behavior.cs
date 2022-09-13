using System;
using System.Collections.Generic;
using RPGProject.GameResources;
using UnityEngine;

namespace RPGProject.Combat
{
    public class Boss0Behavior : MonoBehaviour, IUniqueUnit
    {
        [SerializeField] Ability explosionAbility = null;
        [SerializeField] float percentageOfHealthUntilExplosion = .25f;

        Fighter myFighter = null;
        Health myHealth = null;
        float damageTaken = 0f;
        float damageUntilExplosion = 0f;

        bool hasExplosionQueued = false;

        public void Initialize()
        {
            myFighter = GetComponentInParent<Fighter>();
            myHealth = GetComponentInParent<Health>();
            myHealth.onHealthChange += RecordDamageTaken;

            myFighter.onAbilityUse += ResetExplosionBehavior;

            damageUntilExplosion = myHealth.maxHealthPoints * percentageOfHealthUntilExplosion;
        }

        private void RecordDamageTaken(bool NaN, float _damage)
        {
            if (hasExplosionQueued) return;
            if (_damage > 0) return;
            float damage = Mathf.Abs(_damage);

            damageTaken += damage;

            if(damageTaken > damageUntilExplosion)
            {
                myFighter.selectedAbility = explosionAbility;
                damageTaken = 0;
                hasExplosionQueued = true;
            }
        }

        private void ResetExplosionBehavior()
        {
            if (!hasExplosionQueued) return;
            hasExplosionQueued = false;
        }

        public List<AbilityBehavior> GetAbilityBehaviors()
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