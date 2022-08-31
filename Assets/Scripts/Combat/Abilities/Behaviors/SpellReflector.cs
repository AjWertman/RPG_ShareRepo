using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Behavior that reflects projectiles back to the caster.
    /// </summary>
    public class SpellReflector : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            transform.localEulerAngles = Vector3.zero;
            transform.parent = null;

            targetStatus.ApplyActiveAbilityBehavior(this);
            targetStatus.isReflectingSpells = true;
        }

        public override void OnAbilityDeath()
        {
            targetStatus.isReflectingSpells = false;
            base.OnAbilityDeath();
        }
    }
}
