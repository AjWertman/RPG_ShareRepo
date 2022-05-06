public class SpellReflector : StaticSpell
{
    Fighter targetFighter = null;

    public override void InitalizeSpell(BattleUnit _caster, BattleUnit _target)
    {
        base.InitalizeSpell(_caster, _target);
        targetFighter = target.GetComponent<Fighter>();
        targetFighter.SetIsReflectingSpells(true);
    }

    public override void DestroyStaticSpell()
    {
        targetFighter.SetIsReflectingSpells(false);
        Destroy(gameObject);
    }
}
