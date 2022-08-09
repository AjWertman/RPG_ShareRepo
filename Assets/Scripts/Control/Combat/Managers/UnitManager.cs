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

        public event Action onMoveCompletion;
        public event Action<bool?> onTeamWipe;
        public event Action<UnitController> onUnitDeath;

        public void InitalizeUnitManager()
        {
            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            unitPool = FindObjectOfType<UnitPool>();
            characterMeshPool = FindObjectOfType<CharacterMeshPool>();
            SetupEvents();
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
                List<Fighter> opposingFighters = GetOpposingFighters(isPlayerUnit);

                unit.GetCombatAIBrain().InitalizeAgros(opposingFighters);
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

            UnitInfo unitInfo = unitController.unitInfo;
            unitInfo = new UnitInfo(_unit.unitName, characterKey, _unit.baseLevel,
                _isPlayerTeam, _unit.stats, _unit.basicAttack, _unit.abilities);

            UnitResources unitResources = unitController.unitResources;
            unitResources = new UnitResources(unitController.GetHealth().maxHealthPoints);

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

            unitController.SetupUnitController(unitInfo, unitResources, _startingBlock, _isPlayerTeam, newMesh);

            fighter.unitInfo = unitInfo;
            fighter.unitResources = unitResources;

            unitController.GetCombatAIBrain().InitalizeCombatAIBrain(fighter);

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

        public UnitController GetRandomAlivePlayerUnit()
        {
            List<UnitController> livingPlayerUnits = new List<UnitController>();
            foreach (UnitController playerUnit in playerUnits)
            {
                if (playerUnit.GetHealth().isDead) continue;

                livingPlayerUnits.Add(playerUnit);
            }

            int randomInt = RandomGenerator.GetRandomNumber(0, livingPlayerUnits.Count - 1);
            return playerUnits[randomInt];
        }

        public UnitController GetRandomAliveEnemyUnit()
        {
            List<UnitController> livingEnemyUnits = new List<UnitController>();
            foreach (UnitController enemyUnit in enemyUnits)
            {
                if (enemyUnit.GetHealth().isDead) continue;

                livingEnemyUnits.Add(enemyUnit);
            }

            int randomInt = RandomGenerator.GetRandomNumber(0, livingEnemyUnits.Count - 1);
            return playerUnits[randomInt];
        }

        private List<Fighter> GetOpposingFighters(bool _isPlayerTeam)
        {
            List<UnitController> opposingUnits = new List<UnitController>();
            List<Fighter> opposingFighters = new List<Fighter>();

            if (_isPlayerTeam) opposingUnits = enemyUnits;
            else opposingUnits = playerUnits;

            foreach(UnitController unit in opposingUnits)
            {
                opposingFighters.Add(unit.GetFighter());
            }

            return opposingFighters;
        }

        private void SetUnitTransform(UnitController _unit, GridBlock _startingBlock, bool _isPlayer)
        {
            _unit.transform.position = _startingBlock.travelDestination.position;

            _startingBlock.SetContestedFighter(_unit.GetFighter());

            if (!_isPlayer) _unit.transform.eulerAngles = new Vector3(0, 180, 0);
        }

        private void AddUnitToLists(UnitController _unit, bool _isPlayer)
        {
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

        private void OnUnitDeath(Health _deadUnitHealth)
        {
            UnitController deadUnit = _deadUnitHealth.GetComponent<UnitController>();

            deadUnit.GetUnitUI().ActivateResourceSliders(false);
            TeamWipeCheck();
            onUnitDeath(deadUnit);
        }

        private void TeamWipeCheck()
        {
            if (GetDeadPlayerUnits().Count == startingPlayerTeamSize)
            {
                onTeamWipe(false);
            }
            else if (GetDeadEnemyUnits().Count == startingEnemyTeamSize)
            {
                onTeamWipe(true);
            }
        }

        private void ResetLists()
        {
            unitControllers.Clear();
            playerUnits.Clear();
            enemyUnits.Clear();
        }

        private void SetupEvents()
        {
            foreach (UnitController unit in unitPool.GetAllUnits())
            {
                unit.onMoveCompletion += () => onMoveCompletion();
                unit.GetHealth().onHealthDeath += TestQuestCompletion;
                unit.GetHealth().onAnimDeath += OnUnitDeath;
            }
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
    }
}