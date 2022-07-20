using RPGProject.Combat;
using RPGProject.GameResources;
using RPGProject.Movement;
using RPGProject.Progression;
using RPGProject.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] UnitInfo unitInfo = new UnitInfo();
        [SerializeField] UnitResources unitResources = new UnitResources();
        [SerializeField] Stats startingStats = new Stats();
        
        Animator animator = null;
        Fighter fighter = null;
        ComboLinker comboLinker = null;
        Health health = null;
        Mana mana = null;
        CombatMover mover = null;
 
        CharacterMesh characterMesh = null;
        UnitUI unitUI = null;

        public GridBlock currentBlock = null;

        bool isTurn = false;

        public event Action onMoveCompletion;

        public void InitalizeUnitController()
        {
            animator = GetComponent<Animator>();

            fighter = GetComponent<Fighter>();
            comboLinker = GetComponent<ComboLinker>();
            health = GetComponent<Health>();
            mana = GetComponent<Mana>();
            mover = GetComponent<CombatMover>();
            unitUI = GetComponent<UnitUI>();

            health.InitalizeHealth();
            fighter.InitalizeFighter();
            mover.InitalizeCombatMover();
            unitUI.InitializeUnitUI();

            mover.onDestinationReached += ReachedDestination;
        }

        private void ReachedDestination(GridBlock _newBlock)
        {
            currentBlock.contestedFighter = null;

            currentBlock = _newBlock;

            currentBlock.contestedFighter = fighter;
            onMoveCompletion();
        }

        public void SetupUnitController(UnitInfo _unitInfo, UnitResources _unitResources,
            GridBlock _startingBlock, bool _isPlayer, CharacterMesh _characterMesh)
        {
            SetName(_unitInfo.GetUnitName());
            SetCharacterMesh(_characterMesh);

            comboLinker.SetupComboLinker();

            unitInfo.SetUnitInfo(_unitInfo);
            startingStats.SetStats(unitInfo.GetStats());

            UpdateComponentStats(true);

            if (_isPlayer) SetUnitResources(_unitResources);
            else CalculateResources();

            currentBlock = _startingBlock;
            mover.SetStartVariables(_startingBlock);
            unitUI.SetupUnitUI();
        }

        public IEnumerator UseAbilityBehavior(Fighter _target, Ability _ability)
        {
            unitUI.ActivateUnitIndicator(false);

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

                    float moveDuration = comboLinker.GetFullComboTime(_ability.GetCombo());
                    float moveDurationWOffset = moveDuration * 1.1f;

                    UseAbility(_target, _ability);

                    //AddToCopyList(selectedAbility,isRenCopy);

                    yield return new WaitForSeconds(moveDurationWOffset);

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

                    //DecrementSpellLifetimes
                    yield return new WaitForSeconds(1.5f);

                    fighter.ResetTarget();
                    fighter.ResetAbility();

                    onMoveCompletion();

                    yield break;
                }
            }
        }

        public IEnumerator UseAbilityOnAllBehavior(List<UnitController> _targetTeam, Ability _selectedAbility)
        {
            List<Fighter> targetFighters = new List<Fighter>();

            foreach (UnitController unitController in _targetTeam) targetFighters.Add(unitController.fighter);

            StartCoroutine(fighter.AttackAll(targetFighters, _selectedAbility));
            yield return null;
        }

        public void UseAbility(Fighter _target, Ability _selectedAbility)
        {
            //mana.SpendManaPoints(_selectedAbility.GetManaCost());
            StartCoroutine(fighter.Attack(_target, _selectedAbility));
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

            animator.runtimeAnimatorController = characterMesh.GetAnimatorController();
            animator.avatar = _characterMesh.GetAvatar();

            fighter.SetCharacterMesh(characterMesh);

            characterMesh.gameObject.SetActive(true);
        }

        public void SetUnitResources(UnitResources _unitResources)
        {
            unitResources.SetUnitResources(_unitResources);
            health.SetUnitHealth(unitResources.GetHealthPoints(), unitResources.GetMaxHealthPoints());
            mana.SetMana(unitResources.GetManaPoints(), unitResources.GetMaxManaPoints());
        }

        public void UpdateStats(Stats _updatedStats)
        {
            unitInfo.GetStats().SetStats(_updatedStats);
            UpdateComponentStats(false);
        }

        public void CalculateResources()
        {
            health.CalculateMaxHealthPoints(true);
            mana.CalculateMana(true);

            float healthPoints = health.GetHealthPoints();
            float maxHealthPoints = health.GetMaxHealthPoints();
            float manaPoints = mana.GetManaPoints();
            float maxManaPoints = mana.GetMaxManaPoints();
            UnitResources newUnitResources = new UnitResources();
            newUnitResources.SetUnitResources(healthPoints, maxHealthPoints, manaPoints, maxManaPoints);

            SetUnitResources(newUnitResources);
        }

        public void UpdateComponentStats(bool _isInitialUpdated)
        {
            UpdateFighterStats();
            UpdateHealthStats(_isInitialUpdated);
            UpdateManaStats();
        }

        private void UpdateFighterStats()
        {
            Stats stats = unitInfo.GetStats();
            fighter.UpdateAttributes
                (
                stats.GetStat(StatType.Strength),
                stats.GetStat(StatType.Skill),
                stats.GetStat(StatType.Luck)
                );
        }

        private void UpdateHealthStats(bool _isInitialUpdate)
        {
            Stats stats = unitInfo.GetStats();
            health.UpdateAttributes
                (
               stats.GetStat(StatType.Stamina),
                stats.GetStat(StatType.Armor),
               stats.GetStat(StatType.Resistance)
                );
        }

        private void UpdateManaStats()
        {
            Stats stats = unitInfo.GetStats();
            mana.UpdateAttributes(stats.GetStat(StatType.Spirit));
        }

        public void SetIsTurn(bool _isTurn)
        {
            isTurn = _isTurn;
        }

        public void ResetUnit()
        {
            isTurn = false;
            unitInfo.ResetUnitInfo();
            unitResources.ResetUnitResources();
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
            return unitInfo.GetStats().GetStat(_statType);
        }

        public UnitInfo GetUnitInfo()
        { 
            return unitInfo;
        }

        public UnitResources GetUnitResources()
        {
            return unitResources;
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

        public Animator GetAnimator()
        {
            return animator;
        }

        public CharacterMesh GetCharacterMesh()
        {
            return characterMesh;
        }

        public UnitUI GetUnitUI()
        {
            return unitUI;
        }
    }
}
