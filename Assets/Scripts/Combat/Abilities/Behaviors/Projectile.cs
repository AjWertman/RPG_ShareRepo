using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Behavior that launches itself at a target.
    /// </summary>
    public class Projectile : AbilityBehavior
    {
        [SerializeField] GameObject affectorTrigger = null;
        [SerializeField] float launchForce = 10f;

        public bool isParentAbility = true;

        Transform aimTransform = null;

        bool hasAppliedChangeAmount = false;
        bool isSetup = false;

        private void Update()
        {
            if (isSetup)
            {
                transform.LookAt(aimTransform);
                LaunchProjectile();
            }
        }

        public override void PerformAbilityBehavior()
        {
            if (target == null) return;
            hasAppliedChangeAmount = false;
            aimTransform = target.GetAimTransform();
            isSetup = true;
        }

        public override void OnAbilityDeath()
        {
            if (isParentAbility)
            {
                base.OnAbilityDeath();
            }
        }

        private void LaunchProjectile()
        {
            transform.Translate(Vector3.forward * launchForce * Time.deltaTime);
        }

        private void ReflectProjectile()
        {
            isCritical = false;
            target = caster;
            aimTransform = target.GetAimTransform();
        }

        private void OnTriggerEnter(Collider other)
        {
            CombatTarget hitTarget = other.GetComponent<CombatTarget>();
            SpellReflector hitSpellReflector = other.GetComponent<SpellReflector>();

            Fighter targetFighter = GetTargetFighter();

            if (hitTarget != null && hitTarget == target)
            {
                Fighter hitCombatant = hitTarget.GetComponent<Fighter>();
                if (targetFighter != null || targetFighter != hitCombatant)
                {
                    if(!hasAppliedChangeAmount)
                    {
                        hasAppliedChangeAmount = true;
                        targetFighter.GetHealth().ChangeHealth(changeAmount, isCritical, false);
                    }
                }

                SpawnHitFX(aimTransform.position);                                
                OnAbilityDeath();
                
            }
            else if(hitSpellReflector != null)
            {
                if (hitSpellReflector.caster != targetFighter) return;
                ReflectProjectile();
            }
        }
    }
}