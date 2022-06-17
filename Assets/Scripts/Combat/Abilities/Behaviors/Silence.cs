namespace RPGProject.Combat
{
    public class Silence : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            targetStatus.ApplyActiveAbilityBehavior(this);
            targetStatus.SetIsSilenced(true);
        }

        public override void OnAbilityDeath()
        {
            targetStatus.SetIsSilenced(false);
            base.OnAbilityDeath();
        }
    }
}