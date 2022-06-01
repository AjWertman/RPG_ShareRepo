using RPGProject.Core;
using RPGProject.Movement;
using RPGProject.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class BattleUnit : MonoBehaviour
    {
        [SerializeField] BattleUnitInfo battleUnitInfo = new BattleUnitInfo();
        [SerializeField] BattleUnitResources battleUnitResources = new BattleUnitResources();
        [SerializeField] Stats startingStats;

        [SerializeField] ResourceSlider healthSlider = null;
        [SerializeField] ResourceSlider manaSlider = null;

        Animator animator = null;
        Fighter fighter = null;
        Health health = null;
        Mana mana = null;
        CombatMover mover = null;
        UnitIndicatorUI indicator = null;

        CharacterMesh characterMesh = null;

        bool isTurn = false;

        public event Action onMoveCompletion;

        public void InitalizeBattleUnit()
        {
            animator = GetComponent<Animator>();

            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mana = GetComponent<Mana>();
            mover = GetComponent<CombatMover>();
            indicator = GetComponentInChildren<UnitIndicatorUI>();

            health.onHealthChange += UpdateHealthUI;
            mana.onManaChange += UpdateManaUI;

            fighter.InitalizeFighter();
            mover.InitalizeCombatMover();
        }

        public void SetupBattleUnit(BattleUnitInfo _battleUnitInfo, BattleUnitResources _battleUnitResources,
            bool _isPlayer, CharacterMesh _characterMesh)
        {
            SetName(_battleUnitInfo.GetUnitName());
            SetupIndicator(_isPlayer);
            SetCharacterMesh(_characterMesh);

            battleUnitInfo.SetBattleUnitInfo(_battleUnitInfo);
            startingStats.SetStats(battleUnitInfo.GetStats());

            UpdateComponentStats(true);

            //Refactor?
            if (_isPlayer)
            {
                SetBattleUnitResources(_battleUnitResources);
            }
            else
            {
                CalculateResources();
            }

            mover.SetStartVariables();
        }

        public IEnumerator UseAbilityBehavior(BattleUnit _target, Ability _ability)
        {
            ActivateUnitIndicatorUI(false);

            while (isTurn)
            {
                Fighter targetFighter = _target.GetFighter();

                if (!fighter.IsInRange(_ability.GetAbilityType(), _target)) 
                {
                    Vector3 targetPosition = targetFighter.transform.position;
                    Quaternion currentRotation = transform.rotation;

                    yield return mover.JumpToPos(targetPosition, currentRotation, true);

                    yield return null;
                }
                else
                {
                    if (!_ability.CanTargetAll())
                    {
                        fighter.LookAtTarget(_target.transform);
                    }

                    UseAbility(_target, _ability);

                    //AddToRenCopyList(selectedAbility,isRenCopy);

                    //Refactor - move duration
                    //Bring in combo system
                    yield return new WaitForSeconds(1.5f);

                    if (!health.IsDead())
                    {
                        if (_ability.GetAbilityType() == AbilityType.Melee)
                        {
                            yield return mover.ReturnToStart();
                        }
                        else
                        {
                            transform.localRotation = mover.GetStartRotation();
                            animator.CrossFade("Idle", .1f);
                        }
                    }

                    //currentBattleUnit.DecrementSpellLifetimes();
                    yield return new WaitForSeconds(1.5f);

                    fighter.ResetTarget();
                    fighter.ResetAbility();

                    onMoveCompletion();

                    yield break;
                }
            }
        }

        public void UseAbility(BattleUnit _target, Ability _selectedAbility)
        {
            mana.SpendManaPoints(_selectedAbility.GetManaCost());
            fighter.Attack(_target, _selectedAbility);
        }

        public void SetName(string _name)
        {
            string filteredName = _name.Replace("(Clone)", "");
            name = filteredName;
        }

        public void SetCharacterMesh(CharacterMesh _characterMesh)
        {
            characterMesh = _characterMesh;
            characterMesh.transform.parent = transform;
            characterMesh.transform.localPosition = Vector3.zero;
            characterMesh.transform.localRotation = Quaternion.identity;

            fighter.SetCharacterMesh(characterMesh);

            characterMesh.gameObject.SetActive(true);
        }

        public void SetBattleUnitResources(BattleUnitResources _battleUnitResources)
        {
            battleUnitResources.SetBattleUnitResources(_battleUnitResources);
            health.SetUnitHealth(battleUnitResources.GetHealthPoints(), battleUnitResources.GetMaxHealthPoints());
            mana.SetMana(battleUnitResources.GetManaPoints(), battleUnitResources.GetMaxManaPoints());

            ActivateResourceSliders(true);
        }

        public void UpdateStats(Stats _updatedStats)
        {
            battleUnitInfo.GetStats().SetStats(_updatedStats);
            UpdateComponentStats(false);
        }

        public void CalculateResources()
        {
            health.CalculateMaxHealthPoints(true);
            mana.CalculateMana(true);

            //Refactor
            float healthPoints = health.GetHealthPoints();
            float maxHealthPoints = health.GetMaxHealthPoints();
            float manaPoints = mana.GetManaPoints();
            float maxManaPoints = mana.GetMaxManaPoints();
            BattleUnitResources newBattleUnitResources = new BattleUnitResources();
            newBattleUnitResources.SetBattleUnitResources(healthPoints, maxHealthPoints, manaPoints, maxManaPoints);

            SetBattleUnitResources(newBattleUnitResources);
        }

        public void UpdateComponentStats(bool _isInitialUpdated)
        {
            UpdateFighterStats();
            UpdateHealthStats(_isInitialUpdated);
            UpdateManaStats();
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

        private void UpdateHealthStats(bool _isInitialUpdate)
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

        //Refactor - move to Battle unit UI?
        private void UpdateHealthUI()
        {
            healthSlider.UpdateSliderValue(health.GetHealthPercentage());
        }

        private void UpdateManaUI()
        {
            manaSlider.UpdateSliderValue(mana.GetManaPercentage());
        }

        public void ActivateUnitIndicatorUI(bool _shouldActivate)
        {
            indicator.gameObject.SetActive(_shouldActivate);
        }

        public void SetupIndicator(bool _isPlayer)
        {
            indicator.SetupUI(_isPlayer);
            ActivateUnitIndicatorUI(false);
        }

        public void ActivateResourceSliders(bool _shouldActivate)
        {
            healthSlider.gameObject.SetActive(_shouldActivate);
            manaSlider.gameObject.SetActive(_shouldActivate);
        }

        public void SetIsTurn(bool _isTurn)
        {
            isTurn = _isTurn;
        }

        public void ResetBattleUnit()
        {
            isTurn = false;
            battleUnitInfo.ResetBattleUnitInfo();
            battleUnitResources.ResetBattleUnitResources();
            ResetComponents();
            startingStats.ResetStats();
        }

        private void ResetComponents()
        {
            fighter.ResetFighter();
            health.ResetHealth();
            mana.ResetMana();
        }

        public Stat GetStat(StatType _statType)
        {
            Stat statToGet = null;
            Stats stats = battleUnitInfo.GetStats();
            foreach (Stat stat in stats.GetAllStats())
            {
                if (stat.GetStatType() == _statType)
                {
                    statToGet = stat;
                    break;
                }
            }

            return statToGet;
        }

        public Ability GetRandomAbility()
        {
            Ability basicAttack = battleUnitInfo.GetBasicAttack();
            Ability[] abilities = battleUnitInfo.GetAbilities();

            if (!fighter.IsSilenced())
            {
                if (abilities.Length == 0 || abilities == null) return basicAttack;
                int randomInt = RandomGenerator.GetRandomNumber(0, abilities.Length - 1);
                return abilities[randomInt];
            }
            else
            {
                List<Ability> physicalAbilities = new List<Ability>();
                physicalAbilities.Add(basicAttack);

                foreach (Ability ability in abilities)
                {
                    if (ability.GetAbilityType() == AbilityType.Melee)
                    {
                        physicalAbilities.Add(ability);
                    }
                }

                int randomInt = RandomGenerator.GetRandomNumber(0, physicalAbilities.Count - 1);
                return physicalAbilities[randomInt];
            }
        }

        public List<Ability> GetKnownAbilities()
        {
            List<Ability> useableAbilities = new List<Ability>();
            int currentLevel = battleUnitInfo.GetUnitLevel();

            foreach (Ability ability in battleUnitInfo.GetAbilities())
            {
                if (currentLevel >= ability.GetRequiredLevel())
                {
                    useableAbilities.Add(ability);
                }
            }

            return useableAbilities;
        }

        public BattleUnitInfo GetBattleUnitInfo()
        {
            return battleUnitInfo;
        }

        public BattleUnitResources GetBattleUnitResources()
        {
            return battleUnitResources;
        }

        public CombatMover GetMover()
        {
            return mover;
        }

        public Fighter GetFighter()
        {
            return fighter;
        }

        public Health GetHealth()
        {
            return health;
        }

        public Mana GetMana()
        {
            return mana;
        }

        public CharacterMesh GetCharacterMesh()
        {
            return characterMesh;
        }
    }
}
