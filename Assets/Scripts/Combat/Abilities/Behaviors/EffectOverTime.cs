namespace RPGProject.Combat
{
    public class EffectOverTime : AbilityBehavior
    {
        public override void OnTurnAdvance()
        {
            if (GetTargetFighter() == null) return;
            GetTargetFighter().GetHealthComponent().ChangeHealth(changeAmount, false, true);
            base.OnTurnAdvance();
        }

        public override void PerformAbilityBehavior()
        {
            if (GetTargetFighter() == null) return;
            targetStatus.ApplyActiveAbilityBehavior(this);
        }
    }
}
