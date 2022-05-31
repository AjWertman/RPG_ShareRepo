namespace RPGProject.Combat
{
    public class SpellReflector : AbilityBehavior
    {
        public override void PerformSpellBehavior()
        {
            target.GetFighter().ApplyAbilityBehaviorStatus(this);
            target.GetFighter().SetIsReflectingSpells(true);
        }

        public override void OnAbilityDeath()
        {
            target.GetFighter().SetIsReflectingSpells(false);
            base.OnAbilityDeath();
        }
    }
}
