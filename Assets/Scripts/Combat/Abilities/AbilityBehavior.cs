using System;
using UnityEngine;

namespace RPGProject.Combat
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        protected Ability ability = null;

        protected BattleUnit caster = null;
        protected BattleUnit target = null;
        protected float changeAmount = 0f;
        protected bool isCritical = false;

        protected int abilityLifetime = 0;

        public event Action<AbilityBehavior> onAbilityDeath;

        public void InitializeAbility(Ability _ability)
        {
            ability = _ability;
        }

        public void SetupAbility(BattleUnit _caster, BattleUnit _target, float _changeAmount, bool _isCritical)
        {
            caster = _caster;
            target = _target;
            changeAmount = _changeAmount;
            isCritical = _isCritical;

            abilityLifetime = ability.GetAbilityLifetime();

            transform.position = _caster.GetCharacterMesh().GetLHandTransform().position;
        }

        public abstract void PerformSpellBehavior();

        public virtual void OnAbilityDeath()
        {
            ResetAbilityBehavior();
            onAbilityDeath(this);
        }

        public void DecrementLifetime()
        {
            abilityLifetime--;

            if(abilityLifetime <= 0)
            {
                OnAbilityDeath();
            }
        }

        private void ResetAbilityBehavior()
        {
            caster = null;
            target = null;
            changeAmount = 0;
            isCritical = false;
            abilityLifetime = ability.GetAbilityLifetime();
        }
    }
}