using RPGProject.Combat;
using RPGProject.Combat.Grid;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Questing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    public class UnitManager : MonoBehaviour
    {
        public List<UnitController> unitControllers = new List<UnitController>();
        public List<UnitController> playerUnits = new List<UnitController>();
        public List<UnitController> enemyUnits = new List<UnitController>();

        PlayerTeamManager playerTeamManager = null;

        UnitPool unitPool = null;
        CharacterMeshPool characterMeshPool = null;

        int startingPlayerTeamSize = 0;
        int startingEnemyTeamSize = 0;

        Dictionary<Fighter, UnitAgro> agrosDict = new Dictionary<Fighter, UnitAgro>();

        public event Action onMoveCompletion;
        public event Action<bool?> onTeamWipe;
        public event Action<UnitController> onUnitDeath;

        public void InitalizeUnitManager()
        {
            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            unitPool = FindObjectOfType<UnitPool>();
            characterMeshPool = FindObjectOfType<CharacterMeshPool>();
        }

        public void SetUpUnits(Dictionary<GridBlock, Unit> _playerStartingPositions, Dictionary<GridBlock, Unit> _enemyStartingPositions)
        {
            SetupUnitTeam(_playerStartingPositions, true);
            SetupUnitTeam(_enemyStartingPositions, false);

            startingPlayerTeamSize = playerUnits.Count;
            startingEnemyTeamSize = enemyUnits.Count;

            foreach(UnitController unit in unitControllers)
            {
                bool isPlayerUnit = unit.unitInfo.isPlayer;
                Fighter fighter = unit.GetFighter();
                List<Fighter> opposingFighters = GetOpposingFighters(isPlayerUnit);

                unit.GetUnitAgro().InitalizeAgros(fighter, GetOpposingFighters(isPlayerUnit));
                agrosDict.Add(fighter, unit.GetUnitAgro());

                foreach(IUniqueUnit uniqueUnitBehavior in unit.GetComponentsInChildren<IUniqueUnit>())
                {
                    uniqueUnitBehavior.Initialize();
                }
            }
        }

        public void SetupUnitTeam(Dictionary<GridBlock, Unit> _teamStartingPositions, bool _isPlayerTeam)
        {
            foreach(GridBlock startingBlock in _teamStartingPositions.Keys)
            {
                Unit unit = _teamStartingPositions[startingBlock];
                UnitController unitController = SetupNewUnit(unit, startingBlock, _isPlayerTeam);

                AddUnitToLists(unitController, _isPlayerTeam);
            }
        }

        public void ResetUnitManager()
        {
            characterMeshPool.ResetCharacterMeshPool();
            unitPool.ResetUnitPool();
            ResetLists();
        }

        private UnitController SetupNewUnit(Unit _unit, GridBlock _startingBlock, bool _isPlayerTeam)
        {
            UnitController unitController = unitPool.GetAvailableUnit();
            Fighter fighter = unitController.GetFighter();

            CharacterKey characterKey = _unit.characterKey;

            SetUnitTransform(unitController, _startingBlock, _isPlayerTeam);

            bool isAI = !_isPlayerTeam;
            UnitInfo unitInfo = unitController.unitInfo;
            unitInfo = new UnitInfo(_unit.unitName, characterKey, _unit.baseLevel,
                _isPlayerTeam, isAI, _unit.stats, _unit.basicAttack, _unit.abilities);

            UnitResources unitResources = unitController.unitResources;
            unitResources = new UnitResources(unitController.GetHealth().maxHealthPoints, unitController.GetEnergy().maxEnergyPoints);

            CharacterMesh newMesh = characterMeshPool.GetMesh(characterKey);

            if (_isPlayerTeam)
            {
                PlayerKey playerKey = CharacterKeyComparison.GetPlayerKey(characterKey);

                TeamInfo teamInfo = playerTeamManager.GetTeamInfo(playerKey);
                unitResources = teamInfo.unitResources;
                unitInfo.unitLevel = teamInfo.level;
            }
            else
            {
                //Refactor 
                //Create a xp award script on enemy
                //unitController.SetUnitXPAward(unit.GetXPAward());
            }

            unitController.SetupUnitController(unitInfo, unitResources, _startingBlock, newMesh, true);
            unitController.aiType = _unit.combatAIType;

            unitController.gameObject.SetActive(true);

            return unitController;
        }

        public List<UnitController> GetDeadPlayerUnits()
        {
            List<UnitController> deadPlayerUnits = new List<UnitController>();

            foreach (UnitController playerUnit in playerUnits)
            {
                if (playerUnit.GetHealth().isDead)
                {
                    deadPlayerUnits.Add(playerUnit);
                }
            }

            return deadPlayerUnits;
        }

        public List<UnitController> GetDeadEnemyUnits()
        {
            List<UnitController> deadEnemyUnits = new List<UnitController>();

            foreach (UnitController enemyUnit in enemyUnits)
            {
                if (enemyUnit.GetHealth().isDead)
                {
                    deadEnemyUnits.Add(enemyUnit);
                }
            }

            return deadEnemyUnits;
        }

        public UnitController GetRandomPlayerUnit()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0, playerUnits.Count - 1);

            return playerUnits[randomInt];
        }

        public UnitController GetRandomEnemyUnit()
        {
            int randomInt = RandomGenerator.GetRandomNumber(0, enemyUnits.Count - 1);

            return enemyUnits[randomInt];
        }

        /// <summary>
        /// false = Player team was wiped, 
        /// true = Enemy team was wiped,
        /// null = neither team was wiped
        /// </summary>
        public bool? TeamWipeCheck()
        {
            if (GetDeadPlayerUnits().Count == startingPlayerTeamSize) return false;
            else if (GetDeadEnemyUnits().Count == startingEnemyTeamSize) return true;
            else return null;
        }

        private List<UnitController> GetOpposingUnits(bool _isPlayerTeam)
        {
            List<UnitController> opposingUnits = new List<UnitController>();

            if (_isPlayerTeam) opposingUnits = enemyUnits;
            else opposingUnits = playerUnits;

            return opposingUnits;
        }
        private List<Fighter> GetOpposingFighters(bool _isPlayerTeam)
        {
            List<Fighter> opposingFighters = new List<Fighter>();
            foreach(UnitController opposingUnit in GetOpposingUnits(_isPlayerTeam))
            {
                opposingFighters.Add(opposingUnit.GetFighter());
            }

            return opposingFighters;
        }

        private void SetUnitTransform(UnitController _unit, GridBlock _startingBlock, bool _isPlayer)
        {
            _unit.transform.position = _startingBlock.travelDestination.position;

            _startingBlock.SetContestedFighter(_unit.GetFighter());

            if (!_isPlayer) _unit.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        public void AddUnitToLists(UnitController _unit, bool _isPlayer)
        {
            if (unitControllers.Contains(_unit)) return;

            if (_isPlayer)
            {
                playerUnits.Add(_unit);
            }
            else
            {
                enemyUnits.Add(_unit);
            }

            unitControllers.Add(_unit);
        }

        public void OnUnitAdd(UnitController _newUnit)
        {
            bool isPlayer = _newUnit.unitInfo.isPlayer;
            AddUnitToLists(_newUnit, isPlayer);

            foreach(UnitController opposingUnit in GetOpposingUnits(isPlayer))
            {
                opposingUnit.GetUnitAgro().AddToAgrosList(_newUnit.GetFighter());
            }
        }

        private void OnUnitDeath(Health _deadUnitHealth)
        {
            UnitController deadUnit = _deadUnitHealth.GetComponent<UnitController>();

            TeamWipeCheck();
            onUnitDeath(deadUnit);

            deadUnit.UpdateCurrentBlock(null);
            deadUnit.gameObject.SetActive(false);
        }

        private void ResetLists()
        {
            unitControllers.Clear();
            playerUnits.Clear();
            enemyUnits.Clear();
        }

        //Refactor
        private void TestQuestCompletion(Health _health)
        {
            QuestCompletion myQuestCompletion = _health.GetComponentInChildren<QuestCompletion>();

            if (myQuestCompletion != null)
            {
                myQuestCompletion.CompleteObjective();
            }
        }

        //private void SetupEvents()
        //{
        //    foreach (UnitController unit in unitPool.GetAllUnits())
        //    {
        //        unit.onMoveCompletion += () => onMoveCompletion();
        //        unit.GetHealth().onHealthDeath += TestQuestCompletion;
        //        unit.GetHealth().onAnimDeath += OnUnitDeath;
        //    }
        //}
    }
}