using System;
using UnityEngine;

namespace RPGProject.Combat
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        [SerializeField] protected AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;

        protected Fighter caster = null;
        protected Fighter target = null;
        protected float changeAmount = 0f;
        protected bool isCritical = false;

        public int abilityLifetime = 0;

        public event Action<AbilityBehavior> onAbilityDeath;

        public void SetupAbility(Fighter _caster, Fighter _target, float _changeAmount, bool _isCritical, int _lifeTime)
        {
            caster = _caster;
            target = _target;
            changeAmount = _changeAmount;
            isCritical = _isCritical;
            abilityLifetime = _lifeTime;

            //Refactor - Make way to allow for specific locations
            transform.position = _caster.GetCharacterMesh().GetLHandTransform().position;
        }

        public abstract void PerformSpellBehavior();

        public virtual void OnAbilityDeath()
        {
            ResetAbilityBehavior();
            onAbilityDeath(this);
        }

        public virtual void OnTurnAdvance()
        {
            DecrementLifetime();
        }

        private void DecrementLifetime()
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
            abilityLifetime = 0;
        }
    }
}