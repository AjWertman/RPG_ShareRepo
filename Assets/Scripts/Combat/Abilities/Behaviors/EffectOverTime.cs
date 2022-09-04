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
            DealDamage();
            base.OnTurnAdvance();
        }

        public override void PerformAbilityBehavior()
        {
            if (GetTargetFighter() == null) return;
            DealDamage();

            if(targetStatus!=null) targetStatus.ApplyActiveAbilityBehavior(this);
        }

        public void DealDamage()
        {
            GetTargetFighter().GetHealth().ChangeHealth(changeAmount, false, true);
        }
    }
}
