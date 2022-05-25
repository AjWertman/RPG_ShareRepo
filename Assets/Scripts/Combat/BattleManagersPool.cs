using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManagersPool : MonoBehaviour
{
    [SerializeField] GameObject battlePositionManagerPrefab = null;
    [SerializeField] GameObject battleUnitManagerPrefab = null;
    [SerializeField] GameObject battleUIManagerPrefab = null;
    [SerializeField] GameObject turnManagerPrefab = null;

    BattlePositionManager battlePositionManager = null;
    BattleUnitManager battleUnitManager = null;
    BattleUIManager battleUIManager = null;
    TurnManager turnManager = null;

    private void Start()
    {
        CreateManagers();
    }

    private void CreateManagers()
    {
        GameObject positionInstance = Instantiate(battlePositionManagerPrefab, transform);
        battlePositionManager = positionInstance.GetComponent<BattlePositionManager>();

        GameObject unitInstance = Instantiate(battleUnitManagerPrefab, transform);
        battleUnitManager = unitInstance.GetComponent<BattleUnitManager>();
        battleUnitManager.InitalizeUnitManager();

        GameObject uiInstance = Instantiate(battleUIManagerPrefab, transform);
        battleUIManager = uiInstance.GetComponent<BattleUIManager>();
        battleUIManager.InitalizeBattleUIManager();

        GameObject turnInstance = Instantiate(turnManagerPrefab, transform);
        turnManager = turnInstance.GetComponent<TurnManager>();

        ActivateManagersGameObjects(false);
    }

    public void ActivateManagersPool()
    {
        ActivateManagersGameObjects(true);
    }

    public void ResetManagersPool()
    {
        ResetManagers();
        transform.parent = null;
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        ActivateManagersGameObjects(false);
    }

    private void ResetManagers()
    {
        battlePositionManager.ResetPositionManager();
        battleUnitManager.ResetUnitManager();
        battleUIManager.ResetUIManager();
        turnManager.ResetTurnManager();
    }

    private void ActivateManagersGameObjects(bool _shouldActivate)
    {
        foreach (GameObject gameObject in GetAllManagers())
        {
            gameObject.SetActive(_shouldActivate);
        }
    }

    public BattlePositionManager GetBattlePositionManager()
    {
        return battlePositionManager;
    }

    public BattleUnitManager GetBattleUnitManager()
    {
        return battleUnitManager;
    }

    public BattleUIManager GetBattleUIManager()
    {
        return battleUIManager;
    }

    public TurnManager GetTurnManager()
    {
        return turnManager;
    }

    public IEnumerable<GameObject> GetAllManagers()
    {
        yield return battlePositionManager.gameObject;
        yield return battleUnitManager.gameObject;
        yield return battleUIManager.gameObject;
        yield return turnManager.gameObject;
    }
}
