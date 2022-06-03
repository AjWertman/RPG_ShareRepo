using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Movement;
using RPGProject.Progression;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class UnitController : MonoBehaviour
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

        public IEnumerator UseAbilityBehavior(Fighter _target, Ability _ability)
        {
            ActivateUnitIndicatorUI(false);

            while (isTurn)
            {
                if (!fighter.IsInRange(_ability.GetAbilityType(), _target)) 
                {
                    Vector3 targetPosition = _target.transform.position;
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

        public void UseAbility(Fighter _target, Ability _selectedAbility)
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
                stats.GetStat(StatType.Strength),
                stats.GetStat(StatType.Skill),
                stats.GetStat(StatType.Luck)
                );
        }

        private void UpdateHealthStats(bool _isInitialUpdate)
        {
            Stats stats = battleUnitInfo.GetStats();
            health.UpdateAttributes
                (
               stats.GetStat(StatType.Stamina),
                stats.GetStat(StatType.Armor),
               stats.GetStat(StatType.Resistance)
                );
        }

        private void UpdateManaStats()
        {
            Stats stats = battleUnitInfo.GetStats();
            mana.UpdateAttributes(stats.GetStat(StatType.Spirit));
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

        public int GetStat(StatType _statType)
        {
            return battleUnitInfo.GetStats().GetStat(_statType);
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
