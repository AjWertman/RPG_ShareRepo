using RPGProject.Core;
using RPGProject.GameResources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// Contains the agro for a Unit.
    /// </summary>
    public class UnitAgro : MonoBehaviour
    {
        [SerializeField] int agroPercentagePerDamagePercentage = 1;

        public List<Agro> agros = new List<Agro>();

        Fighter myFighter = null;

        /// <summary>
        /// Sets the average Agro amount based on a number of fighters. 
        /// </summary>
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

        /// <summary>
        /// Changes the agro percentage for a specific fighter, then adds/subtracts that agro amount
        /// equally from the other fighters on the opposing team.
        /// </summary>
        public void UpdateAgro(Fighter _agressor, float _changeAmount)
        {
            Fighter agressorTarget = (Fighter)_agressor.selectedTarget;
            if (myFighter != agressorTarget) return;

            int percentageChange = GetPercentageOfHealthChange(myFighter.GetHealth(), _changeAmount);

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

        /// <summary>
        /// Protects against the total agro from being anything other than 100%
        /// </summary>
        private void FormatAgros(Fighter _agressor)
        {
            int totalPercentageAmount = 0;
            for (int i = 0; i < agros.Count; i++)
            {
                totalPercentageAmount += agros[i].percentageOfAgro;
            }

            int excessPercentage = 0;
            if (totalPercentageAmount > 100)
            {
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

        /// <summary>
        /// Removes a fighter from the agro list (likely caused by death), and redistributes that fighters agro
        /// evenly amongst the other fighters.
        /// </summary>
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

        /// <summary>
        /// Returns the agro percentage for a specific fighter
        /// </summary>
        public int GetAgroPercentage(Fighter _fighter)
        {
            foreach (Agro agro in agros)
            {
                if (agro.fighter == _fighter) return agro.percentageOfAgro;
            }

            return 0;
        }

        /// <summary>
        /// Returns the agro struct from a specific fighter
        /// </summary>
        private Agro GetAgro(Fighter _fighter)
        {
            Agro agroToGet = new Agro();

            foreach (Agro agro in agros)
            {
                if (agro.fighter == _fighter) agroToGet = agro;
            }

            return agroToGet;
        }

        /// <summary>
        /// Gets an evenly split agro percentage based on a number of fighters
        /// </summary>
        private int GetEvenAgroSplit(int _percentageChange)
        {
            int enemyFighterCount = (agros.Count - 1);
            int splitAgro = (_percentageChange / enemyFighterCount);

            return splitAgro;
        }
        
        /// <summary>
        /// Returns the amount of health percentage change to calculate the agro amount.
        /// </summary>
        private int GetPercentageOfHealthChange(Health _health, float _changeAmount)
        {
            bool isDamage = _changeAmount < 0;

            float percentageBeforeDamage = ((_health.healthPoints + Mathf.Abs(_changeAmount)) / _health.maxHealthPoints);
            float percentageAfterDamage = _health.healthPercentage;

            int percentageChangeAmount = (int)((Mathf.Abs(percentageBeforeDamage - percentageAfterDamage)) * 100f);

            return percentageChangeAmount;
        }
    }

    /// <summary>
    /// Struct that contains a fighter and their current percentage of agro.
    /// </summary>
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