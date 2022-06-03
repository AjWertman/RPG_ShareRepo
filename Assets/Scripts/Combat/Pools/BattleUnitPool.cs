using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    //Creates the unit controllers for combat
    public class BattleUnitPool : MonoBehaviour
    {
        [SerializeField] GameObject battleUnitPrefab = null;
        [Range(2, 8)] [SerializeField] int numberOfUnits = 8;

        Dictionary<UnitController, bool> battleUnits = new Dictionary<UnitController, bool>();

        private void Awake()
        {
            CreateBattleUnitPool();
        }

        private void CreateBattleUnitPool()
        {
            for (int i = 0; i < numberOfUnits; i++)
            {
                GameObject battleUnitInstance = Instantiate(battleUnitPrefab, transform);
                UnitController battleUnit = battleUnitInstance.GetComponent<UnitController>();

                battleUnits.Add(battleUnit, false);
                battleUnit.InitalizeBattleUnit();
                battleUnitInstance.gameObject.SetActive(false);
                battleUnit.name = "BattleUnit";
            }
        }

        public UnitController GetAvailableBattleUnit()
        {
            UnitController availableBattleUnit = null;

            foreach (UnitController battleUnit in battleUnits.Keys)
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
            List<UnitController> unitsToReset = new List<UnitController>();

            foreach (UnitController battleUnit in GetActiveBattleUnits())
            {
                battleUnit.transform.parent = transform;
                battleUnit.transform.localPosition = Vector3.zero;
                battleUnit.ResetBattleUnit();
                battleUnit.name = "BattleUnit";
                battleUnit.gameObject.SetActive(false);
                unitsToReset.Add(battleUnit);
            }

            foreach (UnitController battleUnit in unitsToReset)
            {
                battleUnits[battleUnit] = false;
            }
        }

        public IEnumerable<UnitController> GetAllBattleUnits()
        {
            foreach (UnitController battleUnit in battleUnits.Keys)
            {
                yield return battleUnit;
            }
        }

        public IEnumerable<UnitController> GetActiveBattleUnits()
        {
            foreach (UnitController battleUnit in battleUnits.Keys)
            {
                if (battleUnits[battleUnit])
                {
                    yield return battleUnit;
                }
            }
        }
    }
}
