using System;

namespace RPGProject.Combat
{
    [Serializable]
    public struct Agro
    {
        public Fighter fighter;
        public int percentageOfAgro;

        public Agro(Fighter _fighter, int _percentageOfAgro)
        {
            fighter = _fighter;
            percentageOfAgro = _percentageOfAgro;
        }
    }
}