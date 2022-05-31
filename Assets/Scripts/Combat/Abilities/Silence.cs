namespace RPGProject.Combat
{
    public class Silence : AbilityBehavior
    {
        public override void PerformSpellBehavior()
        {
            target.GetFighter().ApplyAbilityBehaviorStatus(this);
            target.GetFighter().SetIsSilenced(true);
        }

        public override void OnAbilityDeath()
        {
            target.GetFighter().SetIsSilenced(false);
            base.OnAbilityDeath();
        }
    }
}