using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.GameResources;
using RPGProject.Movement;
using RPGProject.Progression;
using RPGProject.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class UnitController : MonoBehaviour
    {
        [SerializeField] UnitInfo unitInfo = new UnitInfo();
        [SerializeField] UnitResources unitResources = new UnitResources();
        [SerializeField] Stats startingStats = new Stats();
        
        Animator animator = null;

        public CombatAIBrain combatAIBrain = null;
        CombatMover mover = null;
        Fighter fighter = null;
        ComboLinker comboLinker = null;
        Health health = null;

        CharacterMesh characterMesh = null;
        UnitUI unitUI = null;

        public GridBlock currentBlock = null;
        GridBlock queuedBlock = null;

        bool isTurn = false;

        public event Action onMoveCompletion;
        public event Action<Fighter, GridBlock> onCurrentBlockUpdate;

        public void InitalizeUnitController()
        {
            animator = GetComponent<Animator>();

            combatAIBrain = GetComponent<CombatAIBrain>();
            fighter = GetComponent<Fighter>();
            comboLinker = GetComponent<ComboLinker>();
            health = GetComponent<Health>();
            mover = GetComponent<CombatMover>();
            unitUI = GetComponent<UnitUI>();

            health.InitalizeHealth();
            fighter.InitalizeFighter();
            mover.InitalizeCombatMover();
            unitUI.InitializeUnitUI();

            mover.onBlockReached += UseMovementResources;
        }

        public void SetupUnitController(UnitInfo _unitInfo, UnitResources _unitResources,
            GridBlock _startingBlock, bool _isPlayer, CharacterMesh _characterMesh)
        {
            SetName(_unitInfo.unitName);
            SetCharacterMesh(_characterMesh);

            comboLinker.SetupComboLinker();

            unitInfo = _unitInfo;
            startingStats = unitInfo.stats;

            UpdateComponentStats(true);

            if (_isPlayer) SetUnitResources(_unitResources);
            else CalculateResources();

            currentBlock = _startingBlock;
            unitUI.SetupUnitUI();
        }

        public float GetTotalPossibleGCostAllowance()
        {
            float totalPossibleGCostAllowance = 0f;

            float actionPoints = unitResources.actionPoints;

            totalPossibleGCostAllowance += unitResources.gCostAllowance;
            totalPossibleGCostAllowance += mover.gCostPerAP * actionPoints;

            return totalPossibleGCostAllowance;
        }

        private void UpdateCurrentBlock(GridBlock _newBlock)
        { 
            currentBlock.SetContestedFighter(null);
            currentBlock = _newBlock;
            currentBlock.SetContestedFighter(fighter);
        }

        private void UseMovementResources(float _gCost)
        {
            if (_gCost > unitResources.gCostAllowance)
            {
                _gCost -= unitResources.gCostAllowance;
                unitResources.gCostAllowance = 0;

                unitResources.actionPoints--;
                unitResources.gCostAllowance = mover.gCostPerAP;
            }

            unitResources.gCostAllowance -= _gCost;

            if(unitResources.actionPoints == 0 && unitResources.gCostAllowance < 10)
            {
                onMoveCompletion();
            }
        }

        public IEnumerator PathExecution(List<GridBlock> _path)
        {
            GridBlock goalBlock = _path[_path.Count - 1];
            Fighter contestedFighter = goalBlock.contestedFighter;
            bool isContested = (contestedFighter != null);

            if(isContested)
            {
                _path.Remove(goalBlock);
                goalBlock = _path[_path.Count - 1];
            }

            if(_path.Count > 1)
            {
                List<Transform> travelDestinations = GridSystem.GetTravelDestinations(_path);
                yield return mover.MoveToDestination(travelDestinations);
                UpdateCurrentBlock(goalBlock);
            }

            if (isContested)
            {
                yield return UseAbilityBehavior(contestedFighter, fighter.GetBasicAttack());
            }
        }

        public IEnumerator UseAbilityBehavior(CombatTarget _target, Ability _ability)
        {
            unitUI.ActivateUnitIndicator(false);

            while (isTurn)
            {
                if (!_ability.canTargetAll)
                {
                    fighter.LookAtTarget(_target.GetAimTransform());
                }

                float moveDuration = comboLinker.GetFullComboTime(_ability.combo);
                float moveDurationWOffset = moveDuration * 1.1f;

                UseAbility(_target, _ability);

                //AddToCopyList(selectedAbility,isCopy);

                yield return new WaitForSeconds(moveDurationWOffset);

                animator.CrossFade("Idle", .1f);


                //DecrementSpellLifetimes
                yield return new WaitForSeconds(1.5f);

                fighter.selectedTarget = null;
                fighter.selectedAbility = null;

                onMoveCompletion();
                yield break;
            }
        }

        private void SpendActionPoints(int _apCost)
        {
            //Refactor - does not prevent someone from doing something if they dont have enough ap
            ///Combat Assistant?
            int apAfterSpend = unitResources.actionPoints - _apCost;

            if(apAfterSpend < 0)
            {
                print("Does not have enough points");
                return;
            }

            unitResources.actionPoints -= _apCost;

            if(unitResources.actionPoints == 0)
            {
                onMoveCompletion();
            }
        }
        
        public IEnumerator UseAbilityOnAllBehavior(List<UnitController> _targetTeam, Ability _selectedAbility)
        {
            List<Fighter> targetFighters = new List<Fighter>();

            foreach (UnitController unitController in _targetTeam) targetFighters.Add(unitController.fighter);

            StartCoroutine(fighter.AttackAll(targetFighters, _selectedAbility));
            yield return null;
        }

        public void UseAbility(CombatTarget _target, Ability _selectedAbility)
        {
            //mana.SpendManaPoints(_selectedAbility.GetManaCost());
            StartCoroutine(fighter.Attack(_target, _selectedAbility));
            SpendActionPoints(_selectedAbility.actionPointsCost); 
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

            animator.runtimeAnimatorController = characterMesh.animatorController;
            animator.avatar = _characterMesh.avatar;

            fighter.characterMesh = characterMesh;

            characterMesh.gameObject.SetActive(true);
        }

        public void SetUnitResources(UnitResources _unitResources)
        {
            unitResources = _unitResources;
            health.SetUnitHealth(unitResources.healthPoints, unitResources.maxHealthPoints);
        }

        public void UpdateStats(Stats _updatedStats)
        {
            unitInfo.stats = _updatedStats;
            UpdateComponentStats(false);
        }

        public void CalculateResources()
        {
            health.CalculateMaxHealthPoints(true);

            float healthPoints = health.healthPoints;
            float maxHealthPoints = health.maxHealthPoints;

            UnitResources newUnitResources = new UnitResources(maxHealthPoints);

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
            Stats stats = unitInfo.stats;
            fighter.UpdateAttributes
                (
                stats.GetStatLevel(StatType.Strength),
                stats.GetStatLevel(StatType.Skill),
                stats.GetStatLevel(StatType.Luck)
                );
        }

        private void UpdateHealthStats(bool _isInitialUpdate)
        {
            Stats stats = unitInfo.stats;
            health.UpdateAttributes
                (
               stats.GetStatLevel(StatType.Stamina),
                stats.GetStatLevel(StatType.Armor),
               stats.GetStatLevel(StatType.Resistance)
                );
        }

        private void UpdateManaStats()
        {
            Stats stats = unitInfo.stats;
        }

        public void SetIsTurn(bool _isTurn)
        {
            isTurn = _isTurn;

            if (isTurn)
            {
                unitResources.actionPoints += 4;
            }
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
        }

        public int GetStat(StatType _statType)
        {
            return unitInfo.stats.GetStatLevel(_statType);
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
