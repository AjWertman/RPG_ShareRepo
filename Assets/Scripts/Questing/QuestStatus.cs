using System.Collections.Generic;
using UnityEngine;

public class QuestStatus
{
    [System.Serializable]
    class QuestStatusRecord
    {
        public string questName;
        public Dictionary<string, int> inProgressObjectives;
        public List<string> completedObjectives;
    }

    Quest quest = null;

    Dictionary<string, int> inProgressObjectives = new Dictionary<string, int>();
    List<string> completedObjectives = new List<string>();

    public QuestStatus(Quest quest)
    {
        this.quest = quest;
        foreach(var objective in quest.GetObjectives())
        {
            inProgressObjectives.Add(objective.reference, 0);
        }
    }

    public QuestStatus(object objectState)
    {
        QuestStatusRecord state = objectState as QuestStatusRecord;
        quest = Quest.GetByName(state.questName);
        completedObjectives = state.completedObjectives;
        inProgressObjectives = state.inProgressObjectives;
    }

    public Quest GetQuest()
    {
        return quest;
    }

    public bool IsComplete()
    {
        if (completedObjectives.Count == 0) return false;
        foreach (var objective in quest.GetObjectives())
        {
            string objRef = objective.reference;
            if (!completedObjectives.Contains(objRef))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsObjectiveComplete(string objective)
    {
        return completedObjectives.Contains(objective);
    }

    public void CompleteObjective(string objectiveToComplete)
    {
        if (quest.HasObjective(objectiveToComplete) && !IsObjectiveComplete(objectiveToComplete))
        {           
            inProgressObjectives[objectiveToComplete] = inProgressObjectives[objectiveToComplete] + 1;

            if(inProgressObjectives[objectiveToComplete] == quest.GetObjective(objectiveToComplete).amountToComplete)
            {
                completedObjectives.Add(objectiveToComplete);
            }           
        }
    }

    public int GetCompletedCount()
    {
        return completedObjectives.Count;
    }

    public Dictionary<string, int> GetInProgressObjectives()
    {
        return inProgressObjectives;
    }

    public object CaptureState()
    {
        QuestStatusRecord state = new QuestStatusRecord();

        state.questName = quest.name;
        state.completedObjectives = completedObjectives;
        state.inProgressObjectives = inProgressObjectives;

        return state;
    }
}
