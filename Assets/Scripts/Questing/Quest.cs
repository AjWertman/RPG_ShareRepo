using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/Create New Quest", order = 1)]
public class Quest : ScriptableObject
{
    [System.Serializable]
    public class Objective
    {
        public string reference;
        public int amountToComplete = 1;
        public string description;
        public string[] requiredObjectives;
    }

    [System.Serializable]
    public class Reward
    {
        public int number;
        //public InventoryItem item;
    }

    [SerializeField] string questDescription;

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

    public Objective GetObjective(string objRef)
    {
        foreach(Objective objective in objectives)
        {
            if(objective.reference == objRef)
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

    public bool HasObjective(string objectiveRef)
    {
        foreach(var objective in objectives)
        {
            if(objective.reference == objectiveRef)
            {
                return true;
            }
        }
        return false;
    }

    //public bool HasRequiredAmountOfObjectives(string objectiveRef)
    //{
    //    foreach(Objective objective in objectives)
    //    {
    //        if(objective.reference == objectiveRef)
    //        {
    //            if ()
    //            {
    //                return true;
    //            }
    //            else
    //            {
                    
    //                return false;
    //            }
    //        }
    //    }

    //    return false;
    //}

    public static Quest GetByName(string questName)
    {
        foreach(Quest quest in Resources.LoadAll<Quest>(""))
        {
            if (quest.name == questName)
            {
                return quest;
            }
        }

        return null;
    }
}
