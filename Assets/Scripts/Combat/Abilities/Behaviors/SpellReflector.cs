using UnityEngine;

namespace RPGProject.Combat
{
    public class SpellReflector : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            transform.localEulerAngles = Vector3.zero;
            transform.parent = null;

            targetStatus.ApplyActiveAbilityBehavior(this);
            targetStatus.SetIsReflectingSpells(true);
        }

        public override void OnAbilityDeath()
        {
            targetStatus.SetIsReflectingSpells(false);
            base.OnAbilityDeath();
        }
    }
}
