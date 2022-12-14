using System;

namespace RPGProject.Combat
{
    /// <summary>
    /// The resources that a combatant can use in combat. 
    /// Also used to save the state of resources for player characters when combat is over.
    /// </summary>
    [Serializable]
    public struct UnitResources
    {
        public float healthPoints;
        public float maxHealthPoints;

        public int energyPoints;
        public int maxEnergyPoints;

        public UnitResources(float _maxHealthPoints, int _maxEnergyPoints)
        {
            maxHealthPoints = _maxHealthPoints;
            healthPoints = maxHealthPoints;

            maxEnergyPoints = _maxEnergyPoints;
            energyPoints = maxEnergyPoints;
        }

        public UnitResources(float _healthPoints, float _maxHealthPoints, int _maxEnergyPoints)
        {
            maxHealthPoints = _maxHealthPoints;
            healthPoints = _healthPoints;

            maxEnergyPoints = _maxEnergyPoints;
            energyPoints = maxEnergyPoints;
        }

        public void RestoreHealth()
        {
            healthPoints = maxHealthPoints;
        }

        public void ResetUnitResources()
        {
            healthPoints = 0f;
            maxHealthPoints = 0f;

            energyPoints = 0;
        }
    }
}
