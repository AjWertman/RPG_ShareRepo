using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float force = 10f;

    Ability selectedAbility = null;
    BattleUnit caster = null;
    BattleUnit target = null;
    Transform aimTransform = null;
    
    GameObject hitFX = null;

    bool isSetup = false;

    bool canHitCaster = false;
    bool appliedDamage = false;
    float damageAmount = 0;
    bool isCritical = false;

    private void Update()
    {
        if (isSetup)
        {
            transform.LookAt(aimTransform);
            LaunchProjectile();
        }
    }

    public void SetUpProjectile(Ability abiltyToSet, float _damageAmount, BattleUnit casterToSet, BattleUnit targetToSet, bool _isCritical)
    {
        appliedDamage = false;
        selectedAbility = abiltyToSet;
        damageAmount = _damageAmount;
        caster = casterToSet;
        target = targetToSet;

        if (!target.GetComponent<BattleUnit>().GetFighter().HasSubstitute())
        {
            aimTransform = target.GetComponent<BattleUnit>().GetFighter().GetAimTransform();
        }
        else
        {
            aimTransform = target.GetComponent<BattleUnit>().GetFighter().GetActiveSubstitute().GetFighter().GetAimTransform();
        }
        
        isCritical = _isCritical;
        hitFX = GetHitFX(selectedAbility);
        if (selectedAbility.shouldExpand)
        {
            Vector3 currentScaleAmount = hitFX.transform.lossyScale;
            Vector3 newScaleAmount = target.GetParticleExpander().lossyScale;

            currentScaleAmount = newScaleAmount;
        }

        isSetup = true;
    }

    private GameObject GetHitFX(Ability ability)
    {
        return ability.hitFXPrefab;
    }

    public void LaunchProjectile()
    {
        transform.Translate(Vector3.forward * force * Time.deltaTime);
    }

    public void ReflectProjectile()
    {
        isCritical = false;
        target = caster;
        aimTransform = target.GetComponent<BattleUnit>().GetFighter().GetAimTransform();
        canHitCaster = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BattleUnit>())
        {
            BattleUnit battleUnit = other.GetComponent<BattleUnit>();
            if (battleUnit == target.GetComponent<BattleUnit>())
            {
                Instantiate(hitFX, aimTransform.position, Quaternion.identity);

                if (!appliedDamage)
                {
                   appliedDamage = true;
                   battleUnit.DamageHealth(damageAmount, isCritical, selectedAbility.abilityType);
                }

                Destroy(gameObject);
            }
        }

        if (other.GetComponent<StaticSpell>())
        {
            StaticSpell staticSpell = other.GetComponent<StaticSpell>();

            if (staticSpell.GetStaticSpellType() == StaticSpellType.SpellReflector)
            {
                ReflectProjectile();
                staticSpell.DestroyStaticSpell();
            }
            if (staticSpell.GetStaticSpellType() == StaticSpellType.Substitute)
            {
                staticSpell.DestroyStaticSpell();
                Instantiate(hitFX, aimTransform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}
