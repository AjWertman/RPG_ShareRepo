namespace RPGProject.Combat
{
    public class EffectOverTime : AbilityBehavior
    {
        public override void OnTurnAdvance()
        {
            target.GetHealth().ChangeHealth(changeAmount, false, true);
            base.OnTurnAdvance();
        }

        public override void PerformSpellBehavior()
        {
            target.ApplyActiveAbilityBehavior(this);
        }
    }
}
