using UnityEngine;

namespace RPGProject.Combat
{
    public class SpellReflector : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            transform.localEulerAngles = Vector3.zero;
            transform.parent = null;

            target.ApplyActiveAbilityBehavior(this);
            target.SetIsReflectingSpells(true);
        }

        public override void OnAbilityDeath()
        {
            target.SetIsReflectingSpells(false);
            base.OnAbilityDeath();
        }
    }
}
