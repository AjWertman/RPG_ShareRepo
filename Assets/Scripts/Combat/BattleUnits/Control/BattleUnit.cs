using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] BattleUnitInfo battleUnitInfo = null;
    [SerializeField] BattleUnitResources battleUnitResources = null;

    [SerializeField] GameObject placeholderMesh = null;
    [SerializeField] GameObject unitMesh = null;
    [SerializeField] GameObject unitIndicatorObject = null;

    [SerializeField] GameObject healthSlider = null;
    [SerializeField] GameObject manaSlider = null;

    [SerializeField] Transform particleExpander = null;

    Animator animator = null;
    Fighter fighter = null;
    Health health = null;
    Mana mana = null;
    Mover mover = null;
    UnitIndicatorUI indicator = null;

    //Refactor
    List<GameObject> activeSpellObjects = new List<GameObject>();
    float xpAward = 100f;

    //Make dictionary<BattleUnit, Sprite> for uimanager
    Sprite faceImage = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        fighter = GetComponent<Fighter>();
        health = GetComponent<Health>();
        mana = GetComponent<Mana>();
        mover = GetComponent<Mover>();
        indicator = GetComponentInChildren<UnitIndicatorUI>();

        health.onHealthChange += UpdateHealthUI;
        mana.onManaChange += UpdateManaUI;
        health.onDeath += DestroyAllActiveSpells;
    }

    public void InitalizeBattleUnit(BattleUnitInfo _battleUnitInfo, BattleUnitResources _battleUnitResources, 
        bool _isPlayer, GameObject _unitMesh)
    {
        SetName(_battleUnitInfo.GetUnitName());
        SetupIndicator(_isPlayer);
        SetMesh(_unitMesh);
        SetBattleUnitInfo(_battleUnitInfo);
        UpdateComponentStats(true);
        //Refactor
        if (_isPlayer)
        {
            SetBattleUnitResources(_battleUnitResources);
        }
        else
        {
            CalculateResources();
        }


        mover.SetStartingTransforms();       
    }

    public void SetName(string _name)
    {
        string filteredName = _name.Replace("(Clone)", "");
        name = filteredName;
    }

    public void SetMesh(GameObject _unitMesh)
    {
        _unitMesh.transform.parent = transform;
        _unitMesh.transform.localPosition = Vector3.zero;
        _unitMesh.transform.localRotation = Quaternion.identity;
        _unitMesh.SetActive(true);
    }

    public void SetBattleUnitInfo(BattleUnitInfo _battleUnitInfo)
    {
        battleUnitInfo = _battleUnitInfo;
    }

    public void SetBattleUnitResources(BattleUnitResources _battleUnitResources)
    {
        battleUnitResources = _battleUnitResources;

        float healthPoints = _battleUnitResources.GetHealthPoints();
        float maxHealthPoints = _battleUnitResources.GetMaxHealthPoints();
        float manaPoints = _battleUnitResources.GetManaPoints();
        float maxManaPoints = _battleUnitResources.GetMaxManaPoints();
        health.SetUnitHealth(healthPoints, maxHealthPoints);
        mana.SetMana(manaPoints, maxManaPoints);
    }

    public void SetupIndicator(bool _isPlayer)
    {
        indicator.SetupUI(_isPlayer);
        ActivateUnitIndicatorUI(false);
    }

    public void UpdateStats(Stats _updatedStats)
    {
        battleUnitInfo.SetStats(_updatedStats);
        UpdateComponentStats(false);
    }

    public void CalculateResources()
    {
        health.CalculateMaxHealthPoints(true);
        mana.CalculateMana(true);

        //Refactor
        BattleUnitResources bUR = new BattleUnitResources(health.GetHealthAmount(), health.GetMaxHealthAmount(), mana.GetMana(), mana.GetMaxMana());
        SetBattleUnitResources(bUR);
    }

    public void UpdateComponentStats(bool initialHealthUpdate)
    {
        UpdateFighterStats();
        UpdateHealthStats(initialHealthUpdate);
        UpdateManaStats();
    }


    public void ResetBattleUnit()
    {
        battleUnitInfo.ResetBattleUnitInfo();
        battleUnitResources.ResetBattleUnitResources();
    }

    public BattleUnitInfo GetBattleUnitInfo()
    {
        return battleUnitInfo;
    }

    public BattleUnitResources GetBattleUnitResources()
    {
        return battleUnitResources;
    }

    public Health GetHealth()
    {
        return health;
    }

    public GameObject GetUnitMesh()
    {
        return unitMesh;
    }

    public bool IsIndicatorActive()
    {
        return unitIndicatorObject.activeSelf;
    }

    //Refactor
    public Sprite GetFaceImage()
    {
        return faceImage;
    }
       
    public void SetFaceImage(Sprite _faceImage)
    {
        faceImage = _faceImage;
    }

    public void SetUnitXPAward(float _xpAward)
    {
        xpAward = _xpAward;
    }

    public float GetXPAward()
    {
        return xpAward;
    }
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////



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
        mana.SpendMana(selectedAbility.manaCost);
        fighter.Attack();
    }



    public Ability GetRandomAbility()
    {
        Ability basicAttack = battleUnitInfo.GetBasicAttack();
        Ability[] abilities = battleUnitInfo.GetAbilities();

        if (!fighter.IsSilenced())
        {
            if (abilities.Length == 0 || abilities == null) return basicAttack;
            int randomInt = Random.Range(0, abilities.Length);
            return abilities[randomInt];
        }
        else
        {
            List<Ability> physicalAbilities = new List<Ability>();

            foreach (Ability ability in abilities)
            {
                if(ability.abilityType == AbilityType.Physical)
                {
                    physicalAbilities.Add(ability);
                }
            }

            if(physicalAbilities.Count <= 0)
            {
                return battleUnitInfo.GetBasicAttack();
            }

            int randomInt = Random.Range(0, physicalAbilities.Count);
            return physicalAbilities[randomInt];
        }       
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

    public void SpendMana(float manaCost)
    {
        mana.SpendMana(manaCost);
    }

    public void RestoreMana(float restoreAmount)
    {
        mana.RestoreMana(restoreAmount);
    }

    private void UpdateHealthUI()
    {
        healthSlider.GetComponent<ResourceSlider>().UpdateSliderValue(health.GetHealthPercentage());
    }

    private void UpdateManaUI()
    {
        manaSlider.GetComponent<ResourceSlider>().UpdateSliderValue(mana.GetManaPercentage());
    }

    public void DisableResourceSliders()
    {
        healthSlider.SetActive(false);
        manaSlider.SetActive(false);
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

    public float GetUnitMana()
    {
        return mana.GetMana();
    }

    public float GetUnitMaxMana()
    {
        return mana.GetMaxMana();
    }

    public bool HasEnoughMana(float amountToTest)
    {
        if (mana.GetMana() >= amountToTest)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public GameObject GetPlaceholderMesh()
    {
        return placeholderMesh;
    }

    private void UpdateFighterStats()
    {
        Stats stats = battleUnitInfo.GetStats();
        fighter.UpdateAttributes
            (
            stats.GetSpecificStatLevel(StatType.Strength),
            stats.GetSpecificStatLevel(StatType.Skill),
            stats.GetSpecificStatLevel(StatType.Luck)
            );
    }

    private void UpdateHealthStats(bool initialUpdate)
    {
        Stats stats = battleUnitInfo.GetStats();
        health.UpdateAttributes
            (
            stats.GetSpecificStatLevel(StatType.Stamina),
            stats.GetSpecificStatLevel(StatType.Armor),
            stats.GetSpecificStatLevel(StatType.Resistance)
            );
    }

    private void UpdateManaStats()
    {
        Stats stats = battleUnitInfo.GetStats();
        mana.UpdateAttributes(stats.GetSpecificStatLevel(StatType.Spirit));
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
    //        stats.SetMana(stats.GetMana() + effectAmount);
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
