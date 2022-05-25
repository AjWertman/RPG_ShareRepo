using System;
using System.Collections.Generic;
using UnityEngine;
using AjsUtilityPackage;

public class BattleUnitManager : MonoBehaviour
{
    List<BattleUnit> battleUnits = new List<BattleUnit>();

    List<Unit> playerTeam = new List<Unit>();
    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();

    List<Unit> enemyTeam = new List<Unit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();

    PlayerTeam playerTeamInfo = null;
    BattleUnitPool battleUnitPool = null;
    CharacterMeshPool characterMeshPool = null;
    
    public event Action<bool> onTeamWipe;
    public event Action onUnitListUpdate;

    int startingPlayerTeamSize = 0;
    int startingEnemyTeamSize = 0;

    public void InitalizeUnitManager()
    {
        playerTeamInfo = FindObjectOfType<PlayerTeam>();
        battleUnitPool = FindObjectOfType<BattleUnitPool>();
        characterMeshPool = FindObjectOfType<CharacterMeshPool>();
        SetupOnDeathEvents();
    }

    public void SetUpUnits(List<Unit> _playerTeam, List<Unit> _enemyTeam, List<Transform> _playerPositions, List<Transform> _enemyPositions)
    {
        playerTeam = _playerTeam;
        enemyTeam = _enemyTeam;

        startingPlayerTeamSize = playerTeam.Count;
        startingEnemyTeamSize = enemyTeam.Count;

        SetupUnitTeam(playerTeam, _playerPositions, true);
        SetupUnitTeam(enemyTeam, _enemyPositions, false);
    }

    public void SetupUnitTeam(List<Unit> _team, List<Transform> _teamPositions, bool _isPlayerTeam)
    {
        for (int i = 0; i < _team.Count; i++)
        {
            Unit unit = _team[i];
            BattleUnit battleUnit = SetupNewBattleUnit(unit, _teamPositions[i], _isPlayerTeam);

            AddBattleUnitToLists(battleUnit, _isPlayerTeam);          
        }
    }

    private BattleUnit SetupNewBattleUnit(Unit _unit, Transform _teamPosition, bool _isPlayerTeam)
    {
        BattleUnit battleUnit = battleUnitPool.GetAvailableBattleUnit();

        SetBattleUnitTransform(battleUnit, _teamPosition);

        BattleUnitInfo battleUnitInfo = battleUnit.GetBattleUnitInfo();
        battleUnitInfo.SetBattleUnitInfo(_unit.GetUnitName(), _unit.GetBaseLevel(),
            _isPlayerTeam, _unit.GetBaseStats(), _unit.GetBasicAttack(), _unit.GetAbilities());

        battleUnitInfo.SetFaceImage(_unit.GetFaceImage());

        BattleUnitResources battleUnitResources = battleUnit.GetBattleUnitResources();
        
        GameObject newMesh = characterMeshPool.GetMesh(_unit.GetCharacterMeshKey());

        if (_isPlayerTeam)
        {
            TeamInfo teamInfo = playerTeamInfo.GetTeamInfo(_unit);
            battleUnitInfo.SetUnitLevel(teamInfo.GetLevel());

            battleUnitResources.SetBattleUnitResources(teamInfo.GetBattleUnitResources());
        }
        else
        {
            //Create a xp award script on enemy
            //battleUnit.SetUnitXPAward(unit.GetXPAward());
        }

        battleUnit.SetupBattleUnit(battleUnitInfo, battleUnitResources, _isPlayerTeam, newMesh);
        battleUnit.gameObject.SetActive(true);

        return battleUnit;
    }

    private void SetBattleUnitTransform(BattleUnit _battleUnit, Transform _newTransform)
    {
        _battleUnit.transform.position = _newTransform.position;
        _battleUnit.transform.rotation = _newTransform.rotation;
    }

    private void AddBattleUnitToLists(BattleUnit _battleUnit, bool _isPlayer)
    {
        if (_isPlayer)
        {
            playerUnits.Add(_battleUnit);
        }
        else
        {
            enemyUnits.Add(_battleUnit);
        }

        battleUnits.Add(_battleUnit);
    }

    public void HandlePlayerDeaths()
    {
        foreach (BattleUnit unit in GetAllPlayerUnits())
        {
            if (unit.DeathCheck())
            {
                unit.Die();
            }
        }
    }

    private void OnUnitDeath(BattleUnit _deadUnit)
    {
        bool isPlayer = _deadUnit.GetBattleUnitInfo().IsPlayer();
        if (isPlayer)
        {
            playerUnits.Remove(_deadUnit);
            deadPlayerUnits.Add(_deadUnit);
        }
        else
        {
            enemyUnits.Remove(_deadUnit);
            deadEnemyUnits.Add(_deadUnit);
        }

        TeamWipeCheck();

        onUnitListUpdate();
    }

    private void TeamWipeCheck()
    {
        if (deadPlayerUnits.Count == startingPlayerTeamSize)
        {
            onTeamWipe(false);
        }
        else if (deadEnemyUnits.Count == startingEnemyTeamSize)
        {
            onTeamWipe(true);
        }
    }

    public void ResetUnitManager()
    {
        characterMeshPool.ResetCharacterMeshPool();
        battleUnitPool.ResetBattleUnitPool();
        ResetLists();
    }

    private void ResetLists()
    {
        battleUnits.Clear();

        playerTeam.Clear();
        playerUnits.Clear();
        deadPlayerUnits.Clear();

        enemyTeam.Clear();
        enemyUnits.Clear();
        deadEnemyUnits.Clear();
    }

    private void SetupOnDeathEvents()
    {
        foreach (BattleUnit battleUnit in battleUnitPool.GetAllBattleUnits())
        {
            battleUnit.GetComponent<Health>().onDeath += OnUnitDeath;
        }
    }

    public List<BattleUnit> GetBattleUnits()
    {
        return battleUnits;
    }
    public List<BattleUnit> GetPlayerUnits()
    {
        return playerUnits;
    }
    public List<BattleUnit> GetDeadPlayerUnits()
    {
        return deadPlayerUnits;
    }
    public IEnumerable<BattleUnit> GetAllPlayerUnits()
    {
        foreach (BattleUnit playerUnit in playerUnits)
        {
            yield return playerUnit;
        }
        foreach (BattleUnit playerUnit in deadPlayerUnits)
        {
            yield return playerUnit;
        }
    }
    public List<BattleUnit> GetEnemyUnits()
    {
        return enemyUnits;
    }
    public List<BattleUnit> GetDeadEnemyUnits()
    {
        return deadEnemyUnits;
    }
    public BattleUnit GetRandomPlayerUnit()
    {
        int randomInt = RandomGenerator.GetRandomNumber(0, playerUnits.Count-1);

        return playerUnits[randomInt];
    }
    public BattleUnit GetRandomEnemyUnit()
    {
        int randomInt = RandomGenerator.GetRandomNumber(0, enemyUnits.Count - 1);

        return enemyUnits[randomInt];
    }
}
