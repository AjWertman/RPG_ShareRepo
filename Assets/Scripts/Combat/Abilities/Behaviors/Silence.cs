namespace RPGProject.Combat
{
    /// <summary>
    /// Behavior that prevents a target from using "casting/magical" abilities.
    /// </summary>
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