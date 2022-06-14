using System;
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

            aimTransform = target.GetCharacterMesh().GetAimTransform();

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
            aimTransform = target.GetCharacterMesh().GetAimTransform();
        }

        private void OnTriggerEnter(Collider other)
        {
            Fighter hitCombatant = other.GetComponent<Fighter>();
            SpellReflector hitSpellReflector = other.GetComponent<SpellReflector>();

            //Refactor put Hit FX somewhere

            if (hitCombatant != null && hitCombatant == target)
            {
                if (!hasAppliedChangeAmount)
                {
                    hasAppliedChangeAmount = true;

                    //Add heal check? Will there be healing projectiles?

                    target.GetHealth().ChangeHealth(changeAmount, isCritical, false);

                    
                    OnAbilityDeath();
                }
            }
            else if(hitSpellReflector != null)
            {
                ReflectProjectile();
            }
        }
    }
}