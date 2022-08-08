using UnityEngine;

namespace RPGProject.Combat
{
    public class Projectile : AbilityBehavior
    {
        [SerializeField] float launchForce = 10f;

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
            hasAppliedChangeAmount = false;

            aimTransform = target.GetAimTransform();

            isSetup = true;
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
                        targetFighter.GetHealthComponent().ChangeHealth(changeAmount, isCritical, false);
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