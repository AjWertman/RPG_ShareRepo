using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] GameObject placeholderMesh = null;



    [SerializeField] GameObject unitMesh = null;
    [SerializeField] GameObject unitIndicatorObject = null;
    [SerializeField] Transform particleExpander = null;
   
    [SerializeField] GameObject healthSlider = null;
    [SerializeField] GameObject soulWellSlider = null;
    [SerializeField] bool hasSoulWell = true;

    [SerializeField] int unitLevel = 1;
    [SerializeField] Stats stats;

    float xpAward = 100f;

    string unitName = "";

    Animator animator = null;
    Fighter fighter = null;
    Health health = null;
    SoulWell soulWell = null;
    Mover mover = null;
    UnitIndicatorUI indicator = null;
    SoundFXManager unitSoundFX = null;

    Sprite faceImage = null;

    Stats startingStats;
    Ability basicAttack = null;
    Ability[] spells = null;

    bool isPlayer = true;

    List<GameObject> activeSpellObjects = new List<GameObject>();

    //Initialization///////////////////////////////////////////////////////////////////////////////////////////

    public void SetName(string newName)
    {
        newName = newName.Replace("(Clone)", "");

        unitName = newName;
    }

    public void SetNewMesh(GameObject newMesh)
    {
        newMesh.transform.parent = transform;
        newMesh.transform.localPosition = Vector3.zero;
        newMesh.transform.localRotation = Quaternion.identity;
        newMesh.SetActive(true);
    }

    public void SetupBattleUnitComponents()
    {
        animator = GetComponentInChildren<Animator>();
      
        fighter = GetComponent<Fighter>();
        health = GetComponent<Health>();
        soulWell = GetComponent<SoulWell>();
        mover = GetComponent<Mover>();

        indicator = GetComponentInChildren<UnitIndicatorUI>();
        unitSoundFX = GetComponent<SoundFXManager>();

        health.onDeath += DestroyAllActiveSpells;
    }

    public void SetIsPlayer(bool _isPlayer)
    {
        isPlayer = _isPlayer;
    }

    public void SetFaceImage(Sprite image)
    {
        faceImage = image;
    }

    public Sprite GetFaceImage()
    {
        return faceImage;   
    }

    public void SetStats(Stats _stats)
    {
        stats.SetStats(_stats.GetStats());
        startingStats = stats;
        UpdateAllStats(true);
    }

    public void SetUnitLevel(int level)
    {
        unitLevel = level;
    }

    public void SetUnitXPAward(float _xpAward)
    {
        xpAward = _xpAward;
    }

    public void SetSpells(Ability _basicAttack, Ability[] _spells)
    {
        basicAttack = _basicAttack;
        spells = _spells;
    }

    public void CalculateResources()
    {
        health.CalculateMaxHealthPoints(true);
        soulWell.CalculateSoulWell(true, hasSoulWell);
    }

    public void SetResources(float _healthPoints, float _maxHealthPoints, float _soulWell, float _maxSoulWell)
    {
        health.SetUnitHealth(_healthPoints, _maxHealthPoints);
        soulWell.SetSoulWell(_soulWell, _maxSoulWell);
    }

    public void SetUpResourceSliders()
    {
        health.onHealthChange += UpdateHealthUI;
        soulWell.onSoulWellChange += UpdateSoulWellUI;
    }

    public void SetAnimators()
    {
        health.SetAnimator(animator);
        fighter.SetAnimator(animator);
        mover.SetAnimator(animator, true);
    }

    public void SetUnitSoundFX()
    {
        health.SetUnitSoundFX(unitSoundFX);
        fighter.SetUnitSoundFX(unitSoundFX);
        mover.SetUnitSoundFX(unitSoundFX);
    }

    public void SetupIndicator()
    {
        indicator.SetupUI(isPlayer);
        ActivateUnitIndicatorUI(false);
    }

    public bool IsIndicatorActive()
    {
        return unitIndicatorObject.activeSelf;
    }

    public SoundFXManager GetUnitSoundFX()
    {
        return unitSoundFX;
    }

    public string GetName()
    {
        return unitName;
    }

    public int GetUnitLevel()
    {
        return unitLevel;
    }

    public float GetXPAward()
    {
        return xpAward;
    }

    public GameObject GetUnitMesh()
    {
        return unitMesh;
    }

    public Transform GetParticleExpander()
    {
        return particleExpander;
    }

    //Movement///////////////////////////////////////////////////////////////////////////////////////////

    public Mover GetMover()
    {
        return mover;
    }

    //Combat///////////////////////////////////////////////////////////////////////////////////////////

    public Fighter GetFighter()
    {
        return fighter;
    }

    public void UseAbility(Ability selectedAbility)
    {
        soulWell.SpendSoulWell(selectedAbility.manaCost);
        fighter.Attack();
    }

    public Ability GetBasicAttack()
    {
        return basicAttack;
    }

    public Ability[] GetSpells()
    {
        return spells;
    }

    public Ability GetRandomAbility()
    {
        if (!fighter.IsSilenced())
        {
            if (spells.Length == 0 || spells == null) return basicAttack;
            int randomInt = Random.Range(0, spells.Length);
            return spells[randomInt];
        }
        else
        {
            List<Ability> physicalAbilities = new List<Ability>();
            
            foreach(Ability ability in spells)
            {
                if(ability.abilityType == AbilityType.Physical)
                {
                    physicalAbilities.Add(ability);
                }
            }

            if(physicalAbilities.Count <= 0)
            {
                return basicAttack;
            }

            int randomInt = Random.Range(0, physicalAbilities.Count);
            return physicalAbilities[randomInt];
        }       
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }

    public void ActivateUnitIndicatorUI(bool shouldActivate)
    {
        unitIndicatorObject.SetActive(shouldActivate);
    }

    //UnitResources///////////////////////////////////////////////////////////////////////////////////////////

    public void DamageHealth(float damageAmount, bool isCritical, AbilityType abilityType)
    {
        if (damageAmount <= 0) return;
        StartCoroutine(health.DamageHealth(damageAmount, isCritical, abilityType));   
    }

    public void RestoreHealth( float restoreAmount, bool isCritical)
    {
        StartCoroutine(health.RestoreHealth(restoreAmount, isCritical));      
    }

    public void SpendSoulWell(float soulWellCost)
    {
        soulWell.SpendSoulWell(soulWellCost);
    }

    public void RestoreSoulWell(float restoreAmount)
    {
        soulWell.RestoreSoulWell(restoreAmount);
    }

    private void UpdateHealthUI()
    {
        healthSlider.GetComponent<ResourceSlider>().UpdateSliderValue(health.GetHealthPercentage(), true, null);
    }

    private void UpdateSoulWellUI()
    {
        soulWellSlider.GetComponent<ResourceSlider>().UpdateSliderValue(soulWell.GetSoulWellPercentage(),false, hasSoulWell);
    }

    public void DisableResourceSliders()
    {
        healthSlider.SetActive(false);
        soulWellSlider.SetActive(false);
    }

    public float GetUnitHealth()
    {
        return health.GetHealthAmount();
    }

    public float GetUnitMaxHealth()
    {
        return health.GetMaxHealthAmount();
    }

    public void Die()
    {
        health.Die();
    }

    public bool IsDead()
    {
        return health.IsDead();
    }

    public bool DeathCheck()
    {
        return health.DeathCheck();
    }

    public void AddActiveSpell(GameObject spellToAdd)
    {
        activeSpellObjects.Add(spellToAdd);
    }

    public void DestroyAllActiveSpells(BattleUnit deadUnit)
    {
        foreach (GameObject activeSpell in deadUnit.GetActiveSpells())
        {
            Destroy(activeSpell);
        }
    }

    public List<GameObject> GetActiveSpells()
    {
        return activeSpellObjects;
    }

    public float GetUnitSoulWell()
    {
        return soulWell.GetSoulWell();
    }

    public float GetUnitMaxSoulWell()
    {
        return soulWell.GetMaxSoulWell();
    }

    public bool HasEnoughSoulWell(float amountToTest)
    {
        if (soulWell.GetSoulWell() >= amountToTest)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Stats GetStats()
    {
        return stats;
    }

    public GameObject GetPlaceholderMesh()
    {
        return placeholderMesh;
    }

    private void UpdateAllStats(bool initialHealthUpdate)
    {
        UpdateFighterStats();
        UpdateHealthStats(initialHealthUpdate);
        UpdateManaStats();
    }

    private void UpdateFighterStats()
    {
        fighter.UpdateAttributes
            (
            stats.GetSpecificStatLevel(StatType.Strength),
            stats.GetSpecificStatLevel(StatType.Skill),
            stats.GetSpecificStatLevel(StatType.Luck)
            );
    }

    private void UpdateHealthStats(bool initialUpdate)
    {
        health.UpdateAttributes
            (
            stats.GetSpecificStatLevel(StatType.Stamina),
            stats.GetSpecificStatLevel(StatType.Armor),
            stats.GetSpecificStatLevel(StatType.Resistance)
            );
    }

    private void UpdateManaStats()
    {
        soulWell.UpdateAttributes(stats.GetSpecificStatLevel(StatType.Spirit));
    }

    public void ActivateUnitResourceUI(bool shouldActivate)
    {
        ////////////////////////////////////////////////////////////////////fillin/////////////////////////////////////////////////////
    }

    //Buffs///////////////////////////////////////////////////////////////////////////////////////////

    //public void ApplyBuff(StatBuff buffToApply)
    //{
    //    foreach (Transform buffTransform in buffContainer)
    //    {
    //        StatBuff buffToTest = buffTransform.GetComponent<StatBuff>();

    //        if (buffToApply.affectedStat == buffToTest.GetAffectedStat())
    //        {
    //            if (buffToApply.isDebuff && !buffToTest.GetIsDebuff() || !buffToApply.isDebuff && buffToTest.GetIsDebuff())
    //            {
    //                print("killing");
    //                // have one buff and one debuff of same stat? or cancel out? or take stronger effect?
    //                buffToTest.KillBuff();
    //            }
    //            else
    //            {
    //                print("updating current buff");
    //                bool isNewDuration = false;
    //                int newDuration = 0;

    //                if (buffToApply.duration > buffToTest.GetDuration())
    //                {
    //                    isNewDuration = true;
    //                    newDuration = buffToApply.duration;
    //                }

    //                if (buffToApply.effectAmount > buffToTest.GetEffectAmount())
    //                {
    //                    isNewDuration = true;
    //                    newDuration = buffToApply.duration;
    //                    buffToTest.SetNewEffectAmount(buffToApply.effectAmount);
    //                    StatBuffBehavior(buffToApply);
    //                }

    //                buffToTest.ResetBuffLife(isNewDuration, newDuration);
    //            }
    //        }
    //    }

    //    //else
    //    //    {
    //    //    GameObject buffInstance = Instantiate(buffToApply.buffPrefab, buffContainer);
    //    //    buffInstance.name = buffToApply.buffName;

    //    //    BuffBehavior currentBuffBehavior = buffInstance.GetComponent<BuffBehavior>();
    //    //    currentBuffBehavior.SetupBuff(buffToApply);

    //    //    currentBuffBehavior.onBuffDeath += ResetStat;

    //    //    if (buffToApply.isStatBuff)
    //    //    {
    //    //        StatBuffBehavior(buffToApply);
    //    //    }
    //    //}
    //}

    //private void StatBuffBehavior(StatBuff buffToApply)
    //{
    //    int effectAmount = 0;
    //    if (!buffToApply.isDebuff)
    //    {
    //        effectAmount = buffToApply.effectAmount;
    //    }
    //    else
    //    {
    //        effectAmount = -buffToApply.effectAmount;
    //    }

    //    ManipulateStats(buffToApply.affectedStat, effectAmount);
    //}

    //public void DecrementBuffLifetimes()
    //{
    //    foreach (Transform buffTransform in buffContainer)
    //    {
    //        StatBuff buff = buffTransform.GetComponent<StatBuff>();
    //        buff.DecrementLife();
    //    }
    //}

    //public void DestroyAllBuffs()
    //{
    //    foreach(Transform buff in buffContainer)
    //    {
    //        Destroy(buff);
    //    }
    //}
    
    //Stats///////////////////////////////////////////////////////////////////////////////////////////

    //private void ResetStat(StatAbrv affectedStat, int effectAmount)
    //{
    //    ManipulateStats(affectedStat, effectAmount);
    //}

    //public void ManipulateStats(StatAbrv affectedStat, int effectAmount)
    //{
    //    if (affectedStat == StatAbrv.Str)
    //    {
    //        stats.SetStrength(stats.GetStrength() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Skl)
    //    {
    //        stats.SetSkill(stats.GetSkill() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Stam)
    //    {
    //        stats.SetStamina(stats.GetStamina() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Int)
    //    {
    //        stats.SetSoulWell(stats.GetSoulWell() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Armr)
    //    {
    //        stats.SetArmor(stats.GetArmor() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Res)
    //    {
    //        stats.SetResistance(stats.GetResistance() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Spd)
    //    {
    //        stats.SetSpeed(stats.GetSpeed() + effectAmount);
    //    }
    //    else if (affectedStat == StatAbrv.Lck)
    //    {
    //        stats.SetLuck(stats.GetLuck() + effectAmount);
    //    }

    //    UpdateAllStats(false);
    //}
}
