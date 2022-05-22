using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitPool : MonoBehaviour
{
    [SerializeField] GameObject battleUnitPrefab = null;
    [Range(2, 8)] [SerializeField] int numberOfUnits = 8;

    Dictionary<BattleUnit, bool> battleUnits = new Dictionary<BattleUnit, bool>();

    private void Awake()
    {
        CreateBattleUnitPool();
    }

    private void CreateBattleUnitPool()
    {
        for (int i = 0; i < numberOfUnits; i++)
        {
            GameObject battleUnitInstance = Instantiate(battleUnitPrefab, transform);
            BattleUnit battleUnit = battleUnitInstance.GetComponent<BattleUnit>();

            battleUnits.Add(battleUnit, false);
            battleUnit.GetPlaceholderMesh().SetActive(false);
            battleUnitInstance.gameObject.SetActive(false);
        }
    }

    public BattleUnit GetBattleUnit()
    {
        BattleUnit availableBattleUnit = null;

        foreach (BattleUnit battleUnit in battleUnits.Keys)
        {
            if (battleUnits[battleUnit]) continue;

            availableBattleUnit = battleUnit;
            battleUnits[battleUnit] = true;
            battleUnit.transform.parent = null;
            break;
        }

        return availableBattleUnit;
    }

    public void ResetBattleUnitPool()
    {
        List<BattleUnit> unitsToReset = new List<BattleUnit>();

        foreach (BattleUnit battleUnit in battleUnits.Keys)
        {
            battleUnit.transform.parent = transform;
            battleUnit.transform.localPosition = Vector3.zero;
            battleUnit.ResetBattleUnit();
            battleUnit.gameObject.SetActive(false);
            unitsToReset.Add(battleUnit);
        }

        foreach (BattleUnit battleUnit in unitsToReset)
        {
            battleUnits[battleUnit] = false;
        }
    }

    public IEnumerable<BattleUnit> GetActiveBattleUnits()
    {
        foreach(BattleUnit battleUnit in battleUnits.Keys)
        {
            if (battleUnits[battleUnit])
            {
                yield return battleUnit;
            }
        }
    }
}
