using System;
using UnityEngine;

namespace RPGProject.Combat
{
    public abstract class AbilityBehavior : MonoBehaviour
    {
        [SerializeField] protected AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;

        //protected Ability ability = null;
       
        protected Fighter caster = null;
        protected Fighter target = null;
        protected float changeAmount = 0f;
        protected bool isCritical = false;

        protected float abilityLifetime = 0;

        public event Action<AbilityBehavior> onAbilityDeath;

        //public void InitializeAbility(Ability _ability)
        //{
        //    ability = _ability;
        //    abilityLifetime = ability.GetAbilityLifetime();
        //}

        public void SetupAbility(Fighter _caster, Fighter _target, float _changeAmount, bool _isCritical)
        {
            caster = _caster;
            target = _target;
            changeAmount = _changeAmount;
            isCritical = _isCritical;
            
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
            //abilityLifetime = ability.GetAbilityLifetime();
        }
    }
}