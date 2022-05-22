using AjsUtilityPackage;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitManager : MonoBehaviour
{
    List<BattleUnit> battleUnits = new List<BattleUnit>();

    PlayerTeam playerTeamInfo = null;
    List<Unit> playerTeam = new List<Unit>();
    List<BattleUnit> playerUnits = new List<BattleUnit>();
    List<BattleUnit> deadPlayerUnits = new List<BattleUnit>();

    List<Unit> enemyTeam = new List<Unit>();
    List<BattleUnit> enemyUnits = new List<BattleUnit>();
    List<BattleUnit> deadEnemyUnits = new List<BattleUnit>();

    BattleUnitPool battleUnitPool = null;
    CharacterMeshPool characterMeshPool = null;
    
    public event Action<bool> onTeamWipe;
    public event Action onUnitListUpdate;

    int startingPlayerTeamSize = 0;
    int startingEnemyTeamSize = 0;

    private void Awake()
    {
        battleUnitPool = FindObjectOfType<BattleUnitPool>();
        characterMeshPool = FindObjectOfType<CharacterMeshPool>();
    }

    public void SetUpUnits(PlayerTeam _playerTeamInfo, List<Unit> _enemyTeam, List<Transform> _playerPositions, List<Transform> _enemyPositions)
    {
        playerTeamInfo = _playerTeamInfo;

        playerTeam = playerTeamInfo.GetPlayerTeam();
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
            BattleUnit battleUnit = battleUnitPool.GetBattleUnit();

            AddBattleUnitToLists(battleUnit, _isPlayerTeam);
            SetupBattleUnit(unit, battleUnit, _teamPositions[i], _isPlayerTeam);
        }
    }

    private void SetupBattleUnit(Unit _unit, BattleUnit _battleUnit, Transform _teamPosition, bool _isPlayerTeam)
    {
        SetBattleUnitTransform(_battleUnit, _teamPosition);

        BattleUnitInfo newUnitInfo = CreateNewBattleUnitInfo(_unit, _isPlayerTeam);
        BattleUnitResources newUnitResources = null;
        
        GameObject newMesh = characterMeshPool.GetMesh(_unit.GetCharacterMeshKey());

        if (_isPlayerTeam)
        {
            TeamInfo teamInfo = playerTeamInfo.GetTeamInfo(_unit);
            newUnitInfo.SetUnitLevel(teamInfo.GetLevel());
            newUnitResources = teamInfo.GetBattleUnitResources();
        }
        else
        {
            //Create a xp award script on enemy
            //battleUnit.SetUnitXPAward(unit.GetXPAward());
        }

        _battleUnit.InitalizeBattleUnit(newUnitInfo, newUnitResources, _isPlayerTeam, newMesh);

        //Clear listeners on reset
        _battleUnit.GetHealth().onDeath += OnUnitDeath;

        _battleUnit.gameObject.SetActive(true);
    }


    private BattleUnitInfo CreateNewBattleUnitInfo(Unit _unit, bool _isPlayerTeam)
    {
        BattleUnitInfo newUnitInfo = new BattleUnitInfo();

        string unitName = _unit.GetUnitName();
        int baseLevel = _unit.GetBaseLevel();
        Stats startingStats = _unit.GetBaseStats();
        Ability basicAttack = _unit.GetBasicAttack();
        Ability[] abilities = _unit.GetAbilities();

        newUnitInfo.SetBattleUnitInfo(unitName, baseLevel, _isPlayerTeam, startingStats, basicAttack, abilities);

        return newUnitInfo;
    }

    private void SetupUnitComponents(Unit character, BattleUnit unit, bool isPlayer)
    {
        //unit.SetFaceImage(character.GetFaceImage());     
        //unit.SetActiveSpellContainers();       
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
