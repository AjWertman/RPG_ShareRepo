using System.Collections;
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
        [SerializeField] bool isParabolaThrow = false;

        public bool isParentAbility = true;

        Transform aimTransform = null;
        TrailRenderer trail = null;

        bool hasAppliedChangeAmount = false;
        bool isSetup = false;

        bool isLaunched = false;

        Vector3 launchDirection = Vector3.zero;

        private void Awake()
        {
            trail = GetComponentInChildren<TrailRenderer>();
        }

        private void Update()
        {
            if (isSetup)
            {
                transform.LookAt(aimTransform);

                if (isLaunched) return;
                isLaunched = true;
                StartCoroutine(LaunchProjectile());
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
                if (trail != null) trail.Clear();
                base.OnAbilityDeath();
            }
        }

        private IEnumerator LaunchProjectile()
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.GetAimTransform().position);

            float angle = 45f;
            float gravity = 9.8f;

            if (!isParabolaThrow)
            {
                angle = 0f;
                gravity = 0f;
            }
            float projectileVelocity = distanceToTarget / (Mathf.Sin(2 * angle * Mathf.Deg2Rad) / gravity);

            float velocitySquared = Mathf.Sqrt(projectileVelocity);
            if (velocitySquared <= 0|| float.IsNaN(velocitySquared)) velocitySquared = 1;


            float Vx = velocitySquared * Mathf.Cos(angle * Mathf.Deg2Rad);
            float Vy = velocitySquared * Mathf.Sin(angle * Mathf.Deg2Rad);

            float flightDuration = distanceToTarget / Vx;
            float elapse_time = 0;

            while (elapse_time < flightDuration)
            {
                transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * launchForce * Time.deltaTime);

                elapse_time += Time.deltaTime;

                yield return null;
            }

            //transform.Translate(Vector3.forward * launchForce * Time.deltaTime);
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
                isLaunched = false;
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