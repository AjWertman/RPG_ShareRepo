using UnityEngine;

public class PhysicalReflector : StaticSpell
{
    [SerializeField] float reflectionDamage = 40f;

    Fighter targetFighter = null;

    public override void InitalizeSpell(BattleUnit _caster, BattleUnit _target)
    {
        base.InitalizeSpell(_caster, _target);
        targetFighter = target.GetComponent<Fighter>();
        targetFighter.SetPhysicalReflectionDamage(reflectionDamage);
    }

    public override void DestroyStaticSpell()
    {
        targetFighter.SetPhysicalReflectionDamage(0);
        //onStaticSpellDeath();
        Destroy(gameObject);
    }

    public float GetReflectionDamage()
    {
        return reflectionDamage;
    }

}
