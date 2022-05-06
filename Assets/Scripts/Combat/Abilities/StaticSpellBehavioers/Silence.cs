public class Silence : StaticSpell
{
    Fighter targetFighter = null;

    public override void InitalizeSpell(BattleUnit _caster, BattleUnit _target)
    {
        base.InitalizeSpell(_caster, _target);
        targetFighter = target.GetComponent<Fighter>();
        targetFighter.SetIsSilenced(true);
    }

    public override void DestroyStaticSpell()
    {
        targetFighter.SetIsSilenced(false);
        Destroy(gameObject);
    }
}
