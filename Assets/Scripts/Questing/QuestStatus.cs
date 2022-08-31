using System;
using System.Collections.Generic;

namespace RPGProject.Questing
{
    /// <summary>
    /// Determines the completion of quests and its objectives.
    /// </summary>
    public class QuestStatus
    {     
        public Quest quest = null;

        Dictionary<string, int> inProgressObjectives = new Dictionary<string, int>();
        List<string> completedObjectives = new List<string>();

        public QuestStatus(Quest _quest)
        {
            quest = _quest;
            foreach (var objective in _quest.objectives)
            {
                inProgressObjectives.Add(objective.reference, 0);
            }
        }

        public QuestStatus(object _objectState)
        {
            QuestStatusRecord state = _objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
            inProgressObjectives = state.inProgressObjectives;
        }

        public bool IsComplete()
        {
            if (completedObjectives.Count == 0) return false;
            foreach (var objective in quest.objectives)
            {
                string objRef = objective.reference;
                if (!completedObjectives.Contains(objRef))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsObjectiveComplete(string _objective)
        {
            return completedObjectives.Contains(_objective);
        }

        public void CompleteObjective(string _objectiveToComplete)
        {
            if (quest.HasObjective(_objectiveToComplete) && !IsObjectiveComplete(_objectiveToComplete))
            {
                inProgressObjectives[_objectiveToComplete] = inProgressObjectives[_objectiveToComplete] + 1;

                if (inProgressObjectives[_objectiveToComplete] == quest.GetObjective(_objectiveToComplete).amountToComplete)
                {
                    completedObjectives.Add(_objectiveToComplete);
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

    /// <summary>
    /// Used to save quest statuses
    /// </summary>
    [Serializable]
    class QuestStatusRecord
    {
        public string questName = "";

        public Dictionary<string, int> inProgressObjectives = new Dictionary<string, int>();
        public List<string> completedObjectives = new List<string>();
    }
}
