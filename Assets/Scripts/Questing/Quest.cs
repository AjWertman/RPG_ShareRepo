using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Questing
{
    [CreateAssetMenu(fileName = "Quest", menuName = "Quest/Create New Quest", order = 1)]
    public class Quest : ScriptableObject
    {        
        public string questDescription = "";

        public List<Objective> objectives = new List<Objective>();
        public List<Reward> rewards = new List<Reward>();
        public float xpAward = 100f;

        public string GetTitle()
        {
            return name;
        }

        public Objective GetObjective(string _objectiveRef)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == _objectiveRef)
                {
                    return objective;
                }
            }

            return null;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public bool HasObjective(string _objectiveRef)
        {
            foreach (var objective in objectives)
            {
                if (objective.reference == _objectiveRef)
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
                if (quest.GetTitle() == _questName)
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
        public string reference;
        public int amountToComplete = 1;
        public string description;
        public string[] requiredObjectives;
    }

    [Serializable]
    public class Reward
    {
        public int rewardAmount;
        //public InventoryItem inventoryItem;
    }
}
