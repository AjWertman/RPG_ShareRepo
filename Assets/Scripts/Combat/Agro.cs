using System;

namespace RPGProject.Combat
{
    [Serializable]
    public struct Agro
    {
        public Fighter fighter;
        public float percentageOfAgro;

        public Agro(Fighter _fighter, float _percentageOfAgro)
        {
            fighter = _fighter;
            percentageOfAgro = _percentageOfAgro;
        }

        public void SetPercentageOfAgro(float _newPercentage)
        {
            percentageOfAgro = _newPercentage;
        }
    }
}