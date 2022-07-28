namespace RPGProject.Combat
{
    public class EffectOverTime : AbilityBehavior
    {
        public override void OnTurnAdvance()
        {
            GetTargetFighter().GetHealth().ChangeHealth(changeAmount, false, true);
            base.OnTurnAdvance();
        }

        public override void PerformAbilityBehavior()
        {
            targetStatus.ApplyActiveAbilityBehavior(this);
        }
    }
}
