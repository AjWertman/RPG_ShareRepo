namespace RPGProject.Combat
{
    public class SpellReflector : AbilityBehavior
    {
        public override void PerformSpellBehavior()
        {
            target.ApplyActiveAbilityBehavior(this);
            target.SetIsReflectingSpells(true);
        }

        public override void OnAbilityDeath()
        {
            target.SetIsReflectingSpells(false);
            base.OnAbilityDeath();
        }
    }
}
