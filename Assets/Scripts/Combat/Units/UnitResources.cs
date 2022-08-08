using System;

namespace RPGProject.Combat
{
    [Serializable]
    public struct UnitResources
    {
        public float healthPoints;
        public float maxHealthPoints;

        public int actionPoints;
        public float gCostAllowance;

        public UnitResources(float _maxHealthPoints)
        {
            maxHealthPoints = _maxHealthPoints;
            healthPoints = maxHealthPoints;

            gCostAllowance = 14;
            actionPoints = 0;
        }

        public void RestoreHealth()
        {
            healthPoints = maxHealthPoints;
        }

        public void ResetUnitResources()
        {
            healthPoints = 0f;
            maxHealthPoints = 0f;

            actionPoints = 0;
        }
    }
}
