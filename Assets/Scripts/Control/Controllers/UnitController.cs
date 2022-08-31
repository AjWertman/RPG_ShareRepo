using RPGProject.Combat;
using RPGProject.Combat.AI;
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
    /// <summary>
    /// The instance of a combat unit. Used to control all combat related functions with childed components.
    /// </summary>
    public class UnitController : MonoBehaviour
    {
        public UnitInfo unitInfo = new UnitInfo();
        public UnitResources unitResources = new UnitResources();
        public Stats startingStats = new Stats();

        public AIBattleType aiType = AIBattleType.mDamage;

        public GridBlock currentBlock = null;

        Animator animator = null;
        CombatMover mover = null;
        Fighter fighter = null;
        ComboLinker comboLinker = null;
        Health health = null;
        Energy energy = null;
        CharacterMesh characterMesh = null;
        UnitAgro unitAgro = null;
        UnitUI unitUI = null;
        AimLine aimLine = null;

        GridBlock queuedBlock = null;

        bool isTurn = false;

        public event Action onMoveCompletion;
        public event Action<Fighter, GridBlock> onCurrentBlockUpdate;

        public void InitalizeUnitController()
        {
            fighter = GetComponent<Fighter>();
            comboLinker = GetComponent<ComboLinker>();
            health = GetComponent<Health>();
            energy = GetComponent<Energy>();
            mover = GetComponent<CombatMover>();
            unitAgro = GetComponent<UnitAgro>();
            unitUI = GetComponent<UnitUI>();

            aimLine = GetComponentInChildren<AimLine>();
            aimLine.ResetLine();

            health.InitalizeHealth();
            energy.InitializeEnergy();
            fighter.InitalizeFighter();
            mover.InitalizeCombatMover();
            unitUI.InitializeUnitUI();

            //fighter.onAPUpdate += SetActionPoints;
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

            float energyPoints = energy.energyPoints;

            totalPossibleGCostAllowance += unitResources.gCostMoveAllowance;
            totalPossibleGCostAllowance += mover.gCostPerAP * energyPoints;

            return totalPossibleGCostAllowance;
        }

        public void UpdateCurrentBlock(GridBlock _newBlock)
        { 
            currentBlock.SetContestedFighter(null);
            currentBlock = _newBlock;

            if (currentBlock == null) return;
            currentBlock.SetContestedFighter(fighter);
        }

        private void UseMovementResources(int _gCost)
        {
            int energyCost = 5;

            

            //if (_gCost > unitResources.gCostMoveAllowance)
            //{
            //    _gCost -= unitResources.gCostMoveAllowance;
            //    unitResources.gCostMoveAllowance = 0;

            //    energy.SpendEnergyPoints(1);
            //    unitResources.gCostMoveAllowance = mover.gCostPerAP;
            //}

            //unitResources.gCostMoveAllowance -= _gCost;

            ////if(energy.energyPoints == 0 && unitResources.gCostMoveAllowance < 10)
            ////{
            ////    onMoveCompletion();
            ////}
        }

        public IEnumerator PathExecution(List<GridBlock> _path)
        {
            if (_path == null || _path.Count <= 0) yield break;

            GridBlock goalBlock = _path[_path.Count - 1];
            Fighter contestedFighter = goalBlock.contestedFighter;
            AbilityBehavior activeAbility = goalBlock.activeAbility;

            bool isContested = goalBlock.IsContested(fighter);

            if (isContested)
            {
                _path.Remove(goalBlock);
                goalBlock = _path[_path.Count - 1];
            }

            IEnumerator followPath = FollowPath(_path);

            yield return followPath;

            if (isContested)
            {
                List<CombatTarget> singleTarget = new List<CombatTarget>();
                singleTarget.Add(contestedFighter);

                yield return UseAbilityBehavior(singleTarget, fighter.GetBasicAttack());
            }

            if (unitInfo.isPlayer) onMoveCompletion();
        }

        public IEnumerator FollowPath(List<GridBlock> _path)
        {
            GridBlock goalBlock = _path[_path.Count - 1];

            if (_path.Count > 1)
            {
                List<Transform> travelDestinations = GridSystem.GetTravelDestinations(_path);
                yield return mover.MoveToDestination(travelDestinations);
                UpdateCurrentBlock(goalBlock);
            }
        }

        public IEnumerator UseAbilityBehavior(List<CombatTarget> _targets, Ability _ability)
        {
            unitUI.ActivateUnitIndicator(false);

            while (isTurn)
            {
                yield return UseOnAllTargets(_targets, _ability);

                //DecrementSpellLifetimes

                animator.CrossFade("Idle", .1f);

                yield return new WaitForSeconds(1.5f);

                fighter.selectedTarget = null;
                fighter.selectedAbility = null;

                onMoveCompletion();
                yield break;
            }
        }

        public IEnumerator UseAbilityOnAllBehavior(List<UnitController> _targetTeam, Ability _selectedAbility)
        {
            List<Fighter> targetFighters = new List<Fighter>();

            foreach (UnitController unitController in _targetTeam) targetFighters.Add(unitController.fighter);

            StartCoroutine(fighter.AttackAll(targetFighters, _selectedAbility));
            yield return null;
        }

        private IEnumerator UseOnAllTargets(List<CombatTarget> _targets, Ability _ability)
        {
            List<CombatTarget> privateList = new List<CombatTarget>();
            foreach(CombatTarget target in _targets)
            {
                privateList.Add(target);
            }
            foreach (CombatTarget target in privateList)
            {
                fighter.LookAtTarget(target.GetAimTransform());

                float moveDuration = comboLinker.GetFullComboTime(_ability.combo);
                float moveDurationWOffset = moveDuration * 1.1f;

                UseAbility(target, _ability);

                //AddToCopyList(selectedAbility,isCopy);

                yield return new WaitForSeconds(moveDurationWOffset);
            }
        }

        public void Teleport(GridBlock _newBlock)
        {
            Vector3 teleportDestination = _newBlock.travelDestination.position;
            Vector3 teleportPosition = new Vector3(teleportDestination.x, transform.position.y, teleportDestination.z);
            mover.Teleport(teleportPosition);
            UpdateCurrentBlock(_newBlock);
        }

        private void SpendEnergyPoints(int _energyCost)
        {
            //Refactor - does not prevent someone from doing something if they dont have enough ap
            ///Combat Assistant?
            int apAfterSpend = energy.energyPoints - _energyCost;

            if(apAfterSpend < 0)
            {
                print("Does not have enough points");
                return;
            }

            energy.SpendEnergyPoints(_energyCost);

            if(energy.energyPoints == 0)
            {
                onMoveCompletion();
            }
        }
        

        public void UseAbility(CombatTarget _target, Ability _selectedAbility)
        {
            //mana.SpendManaPoints(_selectedAbility.GetManaCost());
            StartCoroutine(fighter.Attack(_target, _selectedAbility));
            SpendEnergyPoints(_selectedAbility.energyPointsCost); 
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

            SetupAnimator(characterMesh.GetAnimator());

            animator.runtimeAnimatorController = characterMesh.animatorController;
            animator.avatar = _characterMesh.avatar;

            fighter.characterMesh = characterMesh;

            characterMesh.gameObject.SetActive(true);

            characterMesh.InitalizeMesh(gameObject);
        }

        private void SetupAnimator(Animator _animator)
        {
            //Refactor - will this be necessary with new models;
            animator = _animator;
            fighter.SetAnimator(animator);
            mover.SetAnimator(animator);
            health.SetAnimator(animator);
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

            //Refactor - setting player health if already have some damage?
            
            float healthPoints = health.healthPoints;
            float maxHealthPoints = health.maxHealthPoints;

            int maxEnergy = energy.maxEnergyPoints;

            UnitResources newUnitResources = new UnitResources(maxHealthPoints, maxEnergy);

            SetUnitResources(newUnitResources);
        }

        public void UpdateComponentStats(bool _isInitialUpdated)
        {
            UpdateFighterStats();
            UpdateHealthStats(_isInitialUpdated);
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

        public void SetIsTurn(bool _isTurn)
        {
            isTurn = _isTurn;

            if (isTurn)
            {
                //Refactor - How much is restored on new turn? Is it a constant amount or is it based on percentages? Does low health effect this?
                energy.RestoreEnergyPoints(40);
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

        public Energy GetEnergy()
        {
            return energy;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public CharacterMesh GetCharacterMesh()
        {
            return characterMesh;
        }

        public UnitAgro GetUnitAgro()
        {
            return unitAgro;
        }

        public UnitUI GetUnitUI()
        {
            return unitUI;
        }

        public AimLine GetAimLine()
        {
            return aimLine;
        }
    }
}
