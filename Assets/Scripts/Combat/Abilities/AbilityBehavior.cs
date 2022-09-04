using System;
using UnityEngine;

namespace RPGProject.Combat
{
    public enum SpawnLocation { None, LHand, RHand, Target, Target_Parent, Caster, Caster_Parent, Weapon}

    /// <summary>
    /// Base class for an instance of an ability.
    /// </summary>
    public abstract class AbilityBehavior : MonoBehaviour
    {
        [SerializeField] protected AbilityObjectKey abilityObjectKey = AbilityObjectKey.None;
        [SerializeField] protected SpawnLocation spawnLocation = SpawnLocation.None;
        [SerializeField] protected HitFXObjectKey hitFXObjectKey = HitFXObjectKey.None;
        public GridBlockAbiltyKey gridBlockKey = GridBlockAbiltyKey.None;

        public Fighter caster = null;
        public CombatTarget target = null;

        public AbilityBehavior childBehavior = null;

        protected UnitStatus targetStatus = null;
        protected float changeAmount = 0f;
        protected bool isCritical = false;

        protected HitFXPrefab hitFXPrefab = null;

        protected int abilityLifetime = 0;
        protected int numberOfBlocksAffected = 0;

        public event Action<AbilityBehavior> onAbilityDeath;
        
        /// <summary>
        /// Called if the instance requires a hit FX and the location the hit FX should be spawned at
        /// </summary>
        public event Action<HitFXObjectKey, Vector3> hitFXSpawnRequest;

        public void SetupAbility(Fighter _caster, CombatTarget _target, float _changeAmount, bool _isCritical, int _lifeTime)
        {
            caster = _caster;
            target = _target;

            if(_target != null)
            {
                Fighter targetFighter = _target.GetComponent<Fighter>();

                if (targetFighter != null) targetStatus = targetFighter.unitStatus;
            }

            changeAmount = _changeAmount;
            isCritical = _isCritical;
            abilityLifetime = _lifeTime;

            if (caster == null) return;
            SetSpawnLocation(spawnLocation);
        }

        public void SetSpawnLocation(SpawnLocation _spawnLocation)
        {
            if (_spawnLocation == SpawnLocation.None) return;

            CharacterMesh casterMesh = caster.characterMesh;
            Transform aimTransform = target.GetAimTransform();

            switch (_spawnLocation)
            {
                case SpawnLocation.LHand:
                    transform.position = casterMesh.lHandTransform.position;
                    break;

                case SpawnLocation.RHand:
                    transform.position = casterMesh.rHandTransform.position;
                    break;

                case SpawnLocation.Caster:
                    transform.position = caster.transform.position;
                    break;

                case SpawnLocation.Caster_Parent:
                    transform.position = caster.transform.position;
                    transform.parent = caster.transform;
                    break;

                case SpawnLocation.Target:
                    transform.position = aimTransform.position;
                    break;

                case SpawnLocation.Target_Parent:
                    transform.position = aimTransform.position;
                    transform.parent = aimTransform;
                    break;

                case SpawnLocation.Weapon:

                    break;
            }
        }

        /// <summary>
        /// Base behavior for ability instances when they are initialized.
        /// </summary>
        public abstract void PerformAbilityBehavior();

        public virtual void OnAbilityDeath()
        {
            ResetAbilityBehavior();
            onAbilityDeath(this);
        }

        public virtual void OnTurnAdvance()
        {
            DecrementLifetime();
        }

        public string GetAbilityName()
        {
            return abilityObjectKey.ToString();
        }

        protected void SpawnHitFX(Vector3 _position)
        {
            if (hitFXObjectKey == HitFXObjectKey.None) return;
            hitFXSpawnRequest(hitFXObjectKey, _position);
        }

        protected Fighter GetTargetFighter()
        {
            if (target == null) return null;
            return target.GetComponent<Fighter>();
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

    public enum GridBlockAbiltyKey {  None, Fire, }
}