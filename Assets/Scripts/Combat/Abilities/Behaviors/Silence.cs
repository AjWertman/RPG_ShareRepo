namespace RPGProject.Combat
{
    public class Silence : AbilityBehavior
    {
        public override void PerformSpellBehavior()
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