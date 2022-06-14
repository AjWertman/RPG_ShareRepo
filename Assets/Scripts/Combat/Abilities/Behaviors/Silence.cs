namespace RPGProject.Combat
{
    public class Silence : AbilityBehavior
    {
        public override void PerformAbilityBehavior()
        {
            target.ApplyActiveAbilityBehavior(this);
            target.SetIsSilenced(true);
        }

        public override void OnAbilityDeath()
        {
            target.SetIsSilenced(false);
            base.OnAbilityDeath();
        }
    }
}