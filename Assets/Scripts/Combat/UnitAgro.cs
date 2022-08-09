using RPGProject.Core;
using RPGProject.GameResources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class UnitAgro : MonoBehaviour
    {
        [SerializeField] int agroPercentagePerDamagePercentage = 1;

        public List<Agro> agros = new List<Agro>();

        Fighter myFighter = null;

        public void InitalizeAgros(Fighter _myFighter, List<Fighter> _enemyFighters)
        {
            myFighter = _myFighter;
            int initAgroPercentage = 100 / _enemyFighters.Count;
            foreach (Fighter fighter in _enemyFighters)
            {
                Agro newAgro = new Agro(fighter, initAgroPercentage);
                agros.Add(newAgro);

                fighter.onAgroAction += UpdateAgro;
            }
        }

        public void UpdateAgro(Fighter _agressor, float _changeAmount)
        {
            Fighter agressorTarget = (Fighter)_agressor.selectedTarget;
            if (myFighter != agressorTarget) return;

            int percentageChange = GetPercentageOfHealthChange(myFighter.GetHealthComponent(), _changeAmount);

            int agroPercentage = percentageChange * agroPercentagePerDamagePercentage;

            int agroToTakeFromOthers = GetEvenAgroSplit(percentageChange);
            //print("agro to take from others " + agroToTakeFromOthers.ToString());

            for (int i = 0; i < agros.Count; i++)
            {
                Agro agro = agros[i];
                if (agro.fighter == _agressor) agro.percentageOfAgro += percentageChange;
                else agro.percentageOfAgro -= agroToTakeFromOthers;

                agro.percentageOfAgro = Mathf.Clamp(agro.percentageOfAgro, 0, 100);
                agros[i] = agro;
            }

            FormatAgros(_agressor);
        }

        private void FormatAgros(Fighter _agressor)
        {
            int totalPercentageAmount = 0;
            for (int i = 0; i < agros.Count; i++)
            {
                totalPercentageAmount += agros[i].percentageOfAgro;
            }

            //print("total = " + totalPercentageAmount.ToString());

            int excessPercentage = 0;
            if (totalPercentageAmount > 100)
            {
                //print("Total > 100");
                excessPercentage = totalPercentageAmount - 100;

                int splitAgro = GetEvenAgroSplit(excessPercentage);
                for (int i = 0; i < agros.Count; i++)
                {
                    Agro agro = agros[i];
                    agro.percentageOfAgro -= splitAgro;

                    agros[i] = agro;
                }
            }
            else if (totalPercentageAmount < 100)
            {
                //print("Total < 100");
                excessPercentage = 100 - totalPercentageAmount;
                for (int i = 0; i < agros.Count; i++)
                {
                    if (agros[i].fighter == _agressor)
                    {
                        Agro agressorAgro = agros[i];
                        agressorAgro.percentageOfAgro += excessPercentage;
                        agros[i] = agressorAgro;
                    }
                }
            }
        }

        public void RemoveFromAgrosList(Fighter _deadFighter)
        {
            int agroToRedistribute = GetAgroPercentage(_deadFighter);
            agros.Remove(GetAgro(_deadFighter));

            int evenSplit = GetEvenAgroSplit(agroToRedistribute);

            for (int i = 0; i < agros.Count - 1; i++)
            {
                Agro agroToDistributeTo = agros[i];
                agroToDistributeTo.percentageOfAgro += evenSplit;
            }
        }

        public Fighter GetRandomTarget()
        {
            float randomPercentage = RandomGenerator.GetRandomNumber(0f, 100f);
            float currentPercentageMax = 0f;

            foreach (Agro agro in agros)
            {
                currentPercentageMax += agro.percentageOfAgro;

                if (currentPercentageMax > randomPercentage) return agro.fighter;
            }

            return GetHighestAgroFighter();
        }

        public Fighter GetHighestAgroFighter()
        {
            Fighter highestAgroFighter = null;
            float highestPercentage = 0;

            foreach (Agro agro in agros)
            {
                if (agro.percentageOfAgro > highestPercentage)
                {
                    highestAgroFighter = agro.fighter;
                    highestPercentage = agro.percentageOfAgro;
                }
            }

            return highestAgroFighter;
        }

        public int GetAgroPercentage(Fighter _fighter)
        {
            foreach (Agro agro in agros)
            {
                if (agro.fighter == _fighter) return agro.percentageOfAgro;
            }

            return 0;
        }

        private Agro GetAgro(Fighter _fighter)
        {
            Agro agroToGet = new Agro();

            foreach (Agro agro in agros)
            {
                if (agro.fighter == _fighter) agroToGet = agro;
            }

            return agroToGet;
        }

        private int GetEvenAgroSplit(int _percentageChange)
        {
            int enemyFighterCount = (agros.Count - 1);
            int splitAgro = (_percentageChange / enemyFighterCount);

            return splitAgro;
        }

        private int GetPercentageOfHealthChange(Health _health, float _changeAmount)
        {
            bool isDamage = _changeAmount < 0;

            float percentageBeforeDamage = ((_health.healthPoints + Mathf.Abs(_changeAmount)) / _health.maxHealthPoints);
            float percentageAfterDamage = _health.healthPercentage;

            int percentageChangeAmount = (int)((Mathf.Abs(percentageBeforeDamage - percentageAfterDamage)) * 100f);

            return percentageChangeAmount;
        }
    }

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