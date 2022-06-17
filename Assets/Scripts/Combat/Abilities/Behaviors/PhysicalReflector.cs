namespace RPGProject.Combat
{
    public class PhysicalReflector : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            targetStatus.ApplyActiveAbilityBehavior(this);
            targetStatus.SetPhysicalReflectionDamage(changeAmount);
        }

        public override void OnAbilityDeath()
        {
            targetStatus.SetPhysicalReflectionDamage(0);
            base.OnAbilityDeath();
        }
    }
}
