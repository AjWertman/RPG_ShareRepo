namespace RPGProject.Combat
{
    /// <summary>
    /// Behavior that does damage or heals each turn.
    /// </summary>
    public class EffectOverTime : AbilityBehavior
    {
        public override void OnTurnAdvance()
        {
            if (GetTargetFighter() == null) return;
            GetTargetFighter().GetHealth().ChangeHealth(changeAmount, false, true);
            base.OnTurnAdvance();
        }

        public override void PerformAbilityBehavior()
        {
            if (GetTargetFighter() == null) return;
            targetStatus.ApplyActiveAbilityBehavior(this);
        }
    }
}
