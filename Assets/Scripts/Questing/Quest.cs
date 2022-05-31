using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Questing
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest/Create New Quest", order = 1)]
    public class Quest : ScriptableObject
    {        
        [SerializeField] string questDescription = "";

        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();
        [SerializeField] float xpAward = 100f;

        public string GetTitle()
        {
            return name;
        }

        public string GetDescription()
        {
            return questDescription;
        }

        public Objective GetObjective(string _objectiveRef)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.GetReference() == _objectiveRef)
                {
                    return objective;
                }
            }

            return null;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public float GetXPAward()
        {
            return xpAward;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public bool HasObjective(string _objectiveRef)
        {
            foreach (var objective in objectives)
            {
                if (objective.GetReference() == _objectiveRef)
                {
                    return true;
                }
            }
            return false;
        }

        public static Quest GetByName(string _questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == _questName)
                {
                    return quest;
                }
            }

            return null;
        }
    }

    [Serializable]
    public class Objective
    {
        [SerializeField] string reference;
        [SerializeField] int amountToComplete = 1;
        [SerializeField] string description;
        [SerializeField] string[] requiredObjectives;

        public string GetReference()
        {
            return reference;
        }

        public int GetAmountToComplete()
        {
            return amountToComplete;
        }

        public string GetDescription()
        {
            return description;
        }

        public string[] GetRequiredObjectives()
        {
            return requiredObjectives;
        }
    }

    [Serializable]
    public class Reward
    {
        [SerializeField] int rewardAmount;
        //[SerializeField] InventoryItem inventoryItem;

        public int GetRewardAmount()
        {
            return rewardAmount;
        }

        //public InventoryItem GetInventoryItem()
        //{
        //    return inventoryItem;
        //}
    }
}
