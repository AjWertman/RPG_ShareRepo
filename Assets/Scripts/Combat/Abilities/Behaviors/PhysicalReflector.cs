namespace RPGProject.Combat
{
    public class PhysicalReflector : AbilityBehavior
    {
        public override void PerformSpellBehavior()
        {
            target.ApplyActiveAbilityBehavior(this);
            target.SetPhysicalReflectionDamage(changeAmount);
        }

        public override void OnAbilityDeath()
        {
            target.SetPhysicalReflectionDamage(0);
            base.OnAbilityDeath();
        }
    }
}
