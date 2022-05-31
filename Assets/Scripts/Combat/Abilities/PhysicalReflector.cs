namespace RPGProject.Combat
{
    public class PhysicalReflector : AbilityBehavior
    {
        public override void PerformSpellBehavior()
        {
            target.GetFighter().ApplyAbilityBehaviorStatus(this);
            target.GetFighter().SetPhysicalReflectionDamage(changeAmount);
        }

        public override void OnAbilityDeath()
        {
            target.GetFighter().SetPhysicalReflectionDamage(0);
            base.OnAbilityDeath();
        }
    }
}
