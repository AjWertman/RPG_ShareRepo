namespace RPGProject.Combat
{
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
