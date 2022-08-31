namespace RPGProject.Combat
{
    /// <summary>
    /// Behavior that reflects melee damage back to the caster.
    /// </summary>
    public class PhysicalReflector : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            targetStatus.ApplyActiveAbilityBehavior(this);
            targetStatus.physicalReflectionDamage = changeAmount;
        }

        public override void OnAbilityDeath()
        {
            targetStatus.physicalReflectionDamage = 0;
            base.OnAbilityDeath();
        }
    }
}
