namespace RPGProject.Combat
{
    public class Silence : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            targetStatus.ApplyActiveAbilityBehavior(this);
            targetStatus.isSilenced = true;
        }

        public override void OnAbilityDeath()
        {
            targetStatus.isSilenced = false;
            base.OnAbilityDeath();
        }
    }
}