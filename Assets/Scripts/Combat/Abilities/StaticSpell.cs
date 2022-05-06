using UnityEngine;

public enum StaticSpellType { SpellReflector, PhysicalReflector, Substitute, Silence}
public enum StaticSpellLocation { OnTarget, InFrontOfTarget}

public abstract class StaticSpell : MonoBehaviour
{
    [SerializeField] protected StaticSpellType spellType;
    [SerializeField] protected StaticSpellLocation spellLocation;
    [SerializeField] protected bool shouldBeParented = false;

    [SerializeField] public int duration = 0;
    [SerializeField] protected int currentLifetime = 0;

    protected BattleUnit caster = null;
    protected BattleUnit target = null;

    public virtual void InitalizeSpell(BattleUnit _caster, BattleUnit _target)
    {
        caster = _caster;
        target = _target;
        ResetLifetime();
    }

    public abstract void DestroyStaticSpell();

    public void MoveToTransform(Transform transformToMove)
    {
        transform.position = transformToMove.position;
        transform.rotation = transformToMove.rotation;
    }

    public void ResetLifetime()
    {
        currentLifetime = duration;
    }

    public void DecrementLife()
    {
        currentLifetime--;

        if (currentLifetime == 0)
        {
            DestroyStaticSpell();
        }
    }

    public StaticSpellType GetStaticSpellType()
    {
        return spellType;
    }

    public StaticSpellLocation GetStaticSpellLocation()
    {
        return spellLocation;
    }

    public bool ShouldBeParented()
    {
        return shouldBeParented;
    }
}
