using System;
using UnityEngine;

namespace RPGProject.GameResources
{
    public class Energy : MonoBehaviour
    {
        public int energyPoints = 0;
        public int maxEnergyPoints = 100;

        public event Action onEnergyChange;
        public event Action onZeroEnergy;

        public void InitializeEnergy()
        {
            energyPoints = maxEnergyPoints;
        }

        public void SpendEnergyPoints(int _energyToSpend)
        {
            if(energyPoints < _energyToSpend)
            {
                print("Not enough energy points");             
            }

            energyPoints -= _energyToSpend;
            energyPoints = Mathf.Clamp(energyPoints, 0, maxEnergyPoints);

            onEnergyChange();
        }

        public void RestoreEnergyPoints(int _energyToRestore)
        {
            energyPoints += _energyToRestore;
            energyPoints = Mathf.Clamp(energyPoints, 0, maxEnergyPoints);
        }

        public float GetEnergyPercentage()
        {
            float energyPercentage = (float)energyPoints / (float)maxEnergyPoints;
            return energyPercentage;
        }
    }
}