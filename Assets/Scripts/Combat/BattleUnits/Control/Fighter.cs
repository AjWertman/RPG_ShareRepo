using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Fighter : MonoBehaviour
{
    [SerializeField] Transform aimTransform = null;
    [SerializeField] Transform meleeTransform = null;
    [SerializeField] Transform infrontTransform = null;
    [SerializeField] Transform onTargetTransform = null;

    [SerializeField] Transform shootTransform = null;

    Transform buffContainer = null;

    Animator animator = null;
    SoundFXManager unitSoundFX = null;

    Ability selectedAbility = null;

    BattleUnit target = null;
    List<BattleUnit> allTargets = new List<BattleUnit>();

    Substitute activeSubstitute = null;

    float attackRange = 0f;

    float physicalReflectionDamage = 0f;
    bool isReflectingSpells = false;
    bool isSilenced = false;
    bool hasSubstitute = false;

    float strength = 0f;
    float skill = 0f;
    float luck = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpellContainers(Transform buff, Transform staticSpell)
    {
        buffContainer = buff;
    }

    public void SetUnitSoundFX(SoundFXManager _unitSoundFX)
    {
        unitSoundFX = _unitSoundFX;
    }

    public void UpdateAttributes(float _strength, float _skill, float _luck)
    {
        strength = _strength;
        skill = _skill;
        luck = _luck;
    }

    public void Attack()
    {
        //animator.SetFloat("attackSpeed", x);
        animator.CrossFade(selectedAbility.animatorTrigger, .1f);
        //animator.SetTrigger(selectedAbility.animatorTrigger);
    }

    public void Hit()
    {
        bool isCritical = IsCritical();
        
        if (selectedAbility.spellType == SpellType.Ranged)
        {
            CastRangedSpell(isCritical);
            //unitSoundFX.CreateSoundFX(unitSoundFX.GetMagicalAttackSound());
        }

        else if(selectedAbility.spellType == SpellType.Melee)
        {
            CastMeleeAttack(isCritical);
            //unitSoundFX.CreateSoundFX(unitSoundFX.GetPhysAttackSound());
        }

        else if (selectedAbility.spellType == SpellType.Static)
        {
            CastStaticSpell();
            //unitSoundFX.CreateSoundFX(unitSoundFX.GetMagicalAttackSound());
        }

        //if (selectedAbility.buffToApply != null)
        //{
        //ApplyBuff(selectedAbility.buffToApply, target);
        //}
    }

    private void CastRangedSpell(bool isCritical)
    {
        if (!selectedAbility.canTargetAll && !selectedAbility.isHeal)
        {
            GameObject abilityInstance = Instantiate(GetSpellPrefab(), shootTransform.position, Quaternion.identity);
            Projectile instanceProjectile = abilityInstance.GetComponent<Projectile>();
            instanceProjectile.SetUpProjectile
            (
                selectedAbility,
                CalculateChangeAmount(selectedAbility.baseAbilityAmount, isCritical),
                GetComponent<BattleUnit>(),
                target,
                isCritical      
            );
        }
        else if(!selectedAbility.canTargetAll && selectedAbility.isHeal)
        {
            Instantiate(selectedAbility.hitFXPrefab, target.GetFighter().GetAimTransform().position, Quaternion.identity);
            target.RestoreHealth(selectedAbility.baseAbilityAmount, false);
        }
        else
        {
            for (int i = 0; i < allTargets.Count; i++)
            {
                // fix selectewdAbility.hitFXPrefab
                if (!allTargets[i].GetFighter().HasSubstitute())
                {
                    Instantiate(selectedAbility.hitFXPrefab, allTargets[i].GetFighter().GetAimTransform().position, Quaternion.identity);
                    allTargets[i].DamageHealth(CalculateChangeAmount(selectedAbility.baseAbilityAmount, isCritical), isCritical, selectedAbility.abilityType);
                }
                else
                {
                    Instantiate(selectedAbility.hitFXPrefab, allTargets[i].GetFighter().GetActiveSubstitute().transform.position, Quaternion.identity);
                    allTargets[i].GetFighter().GetActiveSubstitute().DestroyStaticSpell();
                }
            }
        }
    }

    private void CastMeleeAttack(bool isCritical)
    {
        if (!target.GetFighter().HasSubstitute())
        {
            target.DamageHealth(CalculateChangeAmount(selectedAbility.baseAbilityAmount, isCritical), isCritical, selectedAbility.abilityType);
        }
        else
        {
            target.GetFighter().GetActiveSubstitute().DestroyStaticSpell();
        }

        HandlePhysicalReflection(target.GetFighter().GetPhysicalReflectionDamage() > 0);
    }

    private void CastStaticSpell()
    {
        GameObject staticSpellInstance = Instantiate(GetSpellPrefab());
        StaticSpell staticSpell = staticSpellInstance.GetComponent<StaticSpell>();
        staticSpell.InitalizeSpell(GetComponent<BattleUnit>(), target);
        target.AddActiveSpell(staticSpellInstance);

        staticSpellInstance.SetActive(false);

        if (selectedAbility.shouldExpand)
        {
            Vector3 newScaleAmount = target.GetParticleExpander().lossyScale;

            staticSpellInstance.GetComponentInChildren<ParticleSystem>().transform.localScale = newScaleAmount;
        }

        staticSpell.MoveToTransform(GetCastLocation(staticSpell.GetStaticSpellLocation()));

        if (staticSpell.ShouldBeParented())
        {
            staticSpell.transform.parent = target.transform;
        }

        staticSpellInstance.SetActive(true);
    }

    private Transform GetCastLocation(StaticSpellLocation location)
    {
        if (!target.GetFighter().HasSubstitute())
        {
            if (location == StaticSpellLocation.OnTarget)
            {
                return target.GetFighter().GetOnTargetTransform();
            }
            else if (location == StaticSpellLocation.InFrontOfTarget)
            {
                return target.GetFighter().GetInFrontTransform();
            }
        }
        else
        {
            Substitute substitute = target.GetFighter().GetActiveSubstitute();
            if (location == StaticSpellLocation.OnTarget)
            {
                //update for substiute;
                return substitute.transform;
            }
            else if (location == StaticSpellLocation.InFrontOfTarget)
            {
                return substitute.GetFighter().GetInFrontTransform();
            }
        }

   
        return null;
    }

    private void HandlePhysicalReflection(bool targetIsReflecting)
    {
        if (!targetIsReflecting) return;

        if (GetComponent<BattleUnit>())
        {
            GetComponent<BattleUnit>().DamageHealth(target.GetComponent<Fighter>().GetPhysicalReflectionDamage(), false, AbilityType.Magical);
        }
        else if (GetComponent<Substitute>())
        {
            GetComponent<Substitute>().DestroyStaticSpell();
        }
    }

    public void SetPhysicalReflectionDamage(float damageToSet)
    {
        physicalReflectionDamage = damageToSet;
    }

    public float GetPhysicalReflectionDamage()
    {
        return physicalReflectionDamage;
    }

    public void SetIsReflectingSpells(bool shouldSet)
    {
        isReflectingSpells = shouldSet;
    }

    public bool IsReflectingSpells()
    {
        return isReflectingSpells;
    }

    public void SetIsSilenced(bool shouldSet)
    {
        isSilenced = shouldSet;
    }

    public bool IsSilenced()
    {
        return isSilenced;
    }

    public void SetHasSubsitute(bool shouldSet)
    {
        hasSubstitute = shouldSet;
    } 

    public bool HasSubstitute()
    {
        return hasSubstitute;
    }

    public void SetActiveSubstitute(Substitute sub)
    {
        activeSubstitute = sub;
    }

    public Substitute GetActiveSubstitute()
    {
        return activeSubstitute;
    }

    public GameObject GetSpellPrefab()
    {
        return selectedAbility.spellPrefab;
    }

    //private void ApplyBuff(StatBuff buffToApply, BattleUnit target)
    //{
    //    target.ApplyBuff(buffToApply);
    //}

    public void SetTarget(BattleUnit _target)
    {
        target = _target;
    }

    public void SetAllTargets(List<BattleUnit> _targets)
    {
        foreach (BattleUnit target in _targets)
        {
            allTargets.Add(target);
        }
    }

    public BattleUnit GetTarget()
    {
        return target;
    }

    public bool IsInRange()
    {
        if (target != null)
        {
            if (!target.GetFighter().HasSubstitute())
            {
                if (GetDistanceToTarget(target.GetFighter().GetMeleeTransform().position) <= attackRange)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (GetDistanceToTarget(target.GetFighter().GetActiveSubstitute().transform.position) <= attackRange)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }          
        }
        else if (allTargets != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float GetDistanceToTarget(Vector3 targetPos)
    {
        return Vector3.Distance(transform.position, targetPos);
    }

    public void LookAtTransform(Transform lookTransform)
    {
        transform.LookAt(lookTransform);
    }

    public bool HasTarget()
    {
        if (target != null || allTargets != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetTarget()
    {
        target = null;

        allTargets.Clear();
    }

    public void SetAbility(Ability _selectedAbility)
    {
        selectedAbility = _selectedAbility;

        SetAttackRange(selectedAbility.spellType == SpellType.Melee);
    }

    public void ResetAbility()
    {
        selectedAbility = null;
    }

    public void SetAttackRange(bool isMelee)
    {
        if (isMelee)
        {
            attackRange = GetComponent<NavMeshAgent>().stoppingDistance;
        }
        else
        {
            attackRange = 1000;
        }
    }

    public float GetAttackRange()
    {
        return attackRange;
    }

    private float CalculateChangeAmount(float _changeAmount, bool isCritical)
    {
        float changeAmount = _changeAmount;
        changeAmount = GetStatsModifier(changeAmount);
        changeAmount += GetCriticalModifier(changeAmount, isCritical);

        return changeAmount;
    }

    private float GetStatsModifier(float changeAmount)
    {
        float newChangeAmount = 0f;
        float statsModifier = 0f;

        if(selectedAbility.abilityType == AbilityType.Physical)
        {
            statsModifier = strength - 10f;
        }
        else if(selectedAbility.abilityType == AbilityType.Magical)
        {
            statsModifier = skill - 10f;
        }

        if (statsModifier == 0)
        {
            newChangeAmount = changeAmount;
        }
        else
        {
            float offensivePercentage = statsModifier * .1f;

            newChangeAmount = changeAmount + (changeAmount * offensivePercentage);
        }

        return newChangeAmount;
    }

    private float GetCriticalModifier(float initalChangeAmount, bool isCritical)
    {
        float criticalModifier = 0;

        if (isCritical)
        {
            criticalModifier = initalChangeAmount / 2;
        }

        return criticalModifier;
    }

    public bool IsCritical()
    {
        float randomFloat = UnityEngine.Random.Range(0, 99);

        if (GetCriticalHitChance() <= randomFloat)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private float GetCriticalHitChance()
    {
        return luck;
    }

    public Transform GetAimTransform()
    {
        return aimTransform;
    }

    public Transform GetMeleeTransform()
    {
        return meleeTransform;
    }

    public Transform GetInFrontTransform()
    {
        return infrontTransform;
    }

    public Transform GetOnTargetTransform()
    {
        return onTargetTransform;
    }
}
