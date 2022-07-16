using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using RPGProject.Questing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    public class UnitManager : MonoBehaviour
    {
        List<UnitController> unitControllers = new List<UnitController>();

        List<Unit> playerTeam = new List<Unit>();
        List<UnitController> playerUnits = new List<UnitController>();

        List<Unit> enemyTeam = new List<Unit>();
        List<UnitController> enemyUnits = new List<UnitController>();

        PlayerTeamManager playerTeamManager = null;

        UnitPool unitPool = null;
        CharacterMeshPool characterMeshPool = null;

        public event Action onMoveCompletion;
        public event Action<bool?> onTeamWipe;
        public event Action<UnitController> onUnitDeath;

        int startingPlayerTeamSize = 0;
        int startingEnemyTeamSize = 0;

        public void InitalizeUnitManager()
        {
            playerTeamManager = FindObjectOfType<PlayerTeamManager>();
            unitPool = FindObjectOfType<UnitPool>();
            characterMeshPool = FindObjectOfType<CharacterMeshPool>();
            SetupEvents();
        }

        public void SetupPlayerUnit(Unit _playerUnit)
        {
            playerTeam.Add(_playerUnit);
        }

        public void SetUpUnits(Dictionary<GridBlock, Unit> _playerStartingPositions, Dictionary<GridBlock, Unit> _enemyStartingPositions)
        {
            SetupUnitTeam(_playerStartingPositions, true);
            SetupUnitTeam(_enemyStartingPositions, false);
        }

        public void SetupUnitTeam(Dictionary<GridBlock, Unit> _teamStartingPositions, bool _isPlayerTeam)
        {
            foreach(GridBlock startingBlock in _teamStartingPositions.Keys)
            {
                Unit unit = _teamStartingPositions[startingBlock];
                UnitController unitController = SetupNewUnit(unit, startingBlock.travelDestination, _isPlayerTeam);

                AddUnitToLists(unitController, _isPlayerTeam);
            }
        }

        private UnitController SetupNewUnit(Unit _unit, Transform _teamPosition, bool _isPlayerTeam)
        {
            UnitController unitController = unitPool.GetAvailableUnit();
            Fighter fighter = unitController.GetFighter();

            CharacterKey characterKey = _unit.GetCharacterKey();

            SetUnitTransform(unitController, _teamPosition, _isPlayerTeam);

            UnitInfo unitInfo = unitController.GetUnitInfo();
            unitInfo.SetUnitInfo(_unit.GetUnitName(), characterKey, _unit.GetBaseLevel(),
                _isPlayerTeam, _unit.GetStats(), _unit.GetBasicAttack(), _unit.GetAbilities());

            UnitResources unitResources = unitController.GetUnitResources();

            CharacterMesh newMesh = characterMeshPool.GetMesh(characterKey);

            if (_isPlayerTeam)
            {
                PlayerKey playerKey = CharacterKeyComparison.GetPlayerKey(characterKey);
                TeamInfo teamInfo = playerTeamManager.GetTeamInfo(playerKey);
                unitResources.SetUnitResources(teamInfo.GetUnitResources());
                unitInfo.SetUnitLevel(teamInfo.GetLevel());
            }
            else
            {
                //Refactor 
                //Create a xp award script on enemy
                //unitController.SetUnitXPAward(unit.GetXPAward());
            }

            unitController.SetupUnitController(unitInfo, unitResources, _isPlayerTeam, newMesh);
            fighter.SetUnitInfo(unitInfo);
            fighter.SetUnitResources(unitResources);
            unitController.gameObject.SetActive(true);

            return unitController;
        }

        private void SetUnitTransform(UnitController _unit, Transform _newTransform, bool _isPlayer)
        {
            _unit.transform.position = _newTransform.position;

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

        public void ResetUnitManager()
        {
            characterMeshPool.ResetCharacterMeshPool();
            unitPool.ResetUnitPool();
            ResetLists();
        }

        private void ResetLists()
        {
            unitControllers.Clear();

            playerTeam.Clear();
            playerUnits.Clear();

            enemyTeam.Clear();
            enemyUnits.Clear();
        }

        private void SetupEvents()
        {
            foreach (UnitController unit in unitPool.GetAllUnits())
            {
                unit.onMoveCompletion += OnMoveCompletion;
                unit.GetHealth().onHealthDeath += TestQuestCompletion;
                unit.GetHealth().onAnimDeath += OnUnitDeath;
            }
        }

        private void TestQuestCompletion(Health _health)
        {
            QuestCompletion myQuestCompletion = _health.GetComponentInChildren<QuestCompletion>();

            if (myQuestCompletion != null)
            {
                myQuestCompletion.CompleteObjective();
            }
        }

        public void OnMoveCompletion()
        {
            onMoveCompletion();
        }

        public List<UnitController> GetAllUnits()
        {
            return unitControllers;
        }

        public List<UnitController> GetPlayerUnits()
        {
            return playerUnits;
        }

        public List<UnitController> GetDeadPlayerUnits()
        {
            List<UnitController> deadPlayerUnits = new List<UnitController>();

            foreach (UnitController playerUnit in playerUnits)
            {
                if (playerUnit.GetHealth().IsDead())
                {
                    deadPlayerUnits.Add(playerUnit);
                }
            }

            return deadPlayerUnits;
        }

        public List<UnitController> GetEnemyUnits()
        {
            return enemyUnits;
        }

        public List<UnitController> GetDeadEnemyUnits()
        {
            List<UnitController> deadEnemyUnits = new List<UnitController>();

            foreach (UnitController enemyUnit in enemyUnits)
            {
                if (enemyUnit.GetHealth().IsDead())
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
            foreach(UnitController playerUnit in playerUnits)
            {
                if (playerUnit.GetHealth().IsDead()) continue;

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
                if (enemyUnit.GetHealth().IsDead()) continue;

                livingEnemyUnits.Add(enemyUnit);
            }

            int randomInt = RandomGenerator.GetRandomNumber(0, livingEnemyUnits.Count - 1);
            return playerUnits[randomInt];
        }
    }
}