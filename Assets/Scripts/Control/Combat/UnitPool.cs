using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control.Combat
{
    /// <summary>
    /// Pools UnitControllers to be used in combat.
    /// </summary>
    public class UnitPool : MonoBehaviour
    {
        [SerializeField] GameObject unitPrefab = null;
        [Range(2, 8)] [SerializeField] int numberOfUnits = 8;

        Dictionary<UnitController, bool> unitsControllers = new Dictionary<UnitController, bool>();

        private void Awake()
        {
            CreateUnitPool();
        }

        private void CreateUnitPool()
        {
            for (int i = 0; i < numberOfUnits; i++)
            {
                GameObject unitInstance = Instantiate(unitPrefab, transform);
                UnitController unitController = unitInstance.GetComponent<UnitController>();

                unitsControllers.Add(unitController, false);
                unitController.InitalizeUnitController();
                unitInstance.gameObject.SetActive(false);
                unitController.name = "Unit";
            }
        }

        public UnitController GetAvailableUnit()
        {
            UnitController availableUnit = null;

            foreach (UnitController unit in unitsControllers.Keys)
            {
                if (unitsControllers[unit]) continue;

                availableUnit = unit;
                unitsControllers[unit] = true;
                unit.transform.parent = null;
                break;
            }

            return availableUnit;
        }

        public void ResetUnitPool()
        {
            List<UnitController> unitsToReset = new List<UnitController>();

            foreach (UnitController unit in GetActiveUnits())
            {
                unit.transform.parent = transform;
                unit.transform.localPosition = Vector3.zero;
                unit.ResetUnit();
                unit.name = "Unit";
                unit.gameObject.SetActive(false);
                unitsToReset.Add(unit);
            }

            foreach (UnitController unit in unitsToReset)
            {
                unitsControllers[unit] = false;
            }
        }

        public IEnumerable<UnitController> GetAllUnits()
        {
            foreach (UnitController unit in unitsControllers.Keys)
            {
                yield return unit;
            }
        }

        public IEnumerable<UnitController> GetActiveUnits()
        {
            foreach (UnitController unit in unitsControllers.Keys)
            {
                if (unitsControllers[unit])
                {
                    yield return unit;
                }
            }
        }
    }
}
