using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Questing
{
    public class QuestStatus
    {     
        Quest quest = null;

        Dictionary<string, int> inProgressObjectives = new Dictionary<string, int>();
        List<string> completedObjectives = new List<string>();

        public QuestStatus(Quest _quest)
        {
            quest = _quest;
            foreach (var objective in _quest.GetObjectives())
            {
                inProgressObjectives.Add(objective.GetReference(), 0);
            }
        }

        public QuestStatus(object _objectState)
        {
            QuestStatusRecord state = _objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.GetQuestName());
            completedObjectives = state.GetCompletedObjectives();
            inProgressObjectives = state.GetInProgressObjectives();
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
                string objRef = objective.GetReference();
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

                if (inProgressObjectives[_objectiveToComplete] == quest.GetObjective(_objectiveToComplete).GetAmountToComplete())
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

            state.SetQuestName(quest.name);
            state.SetCompletedObjectives(completedObjectives);
            state.SetInProgressObjectives(inProgressObjectives);

            return state;
        }
    }

    [Serializable]
    class QuestStatusRecord
    {
        [SerializeField] string questName = "";
        [SerializeField] Dictionary<string, int> inProgressObjectives = new Dictionary<string, int>();
        [SerializeField] List<string> completedObjectives = new List<string>();

        public void SetQuestName(string _questName)
        {
            questName = _questName;
        }

        public void SetInProgressObjectives(Dictionary<string, int> _inProgressObjectives)
        {
            inProgressObjectives = _inProgressObjectives;
        }

        public void SetCompletedObjectives(List<string> _completedObjectives)
        {
            completedObjectives = _completedObjectives;
        }

        public string GetQuestName()
        {
            return questName;
        }

        public Dictionary<string, int> GetInProgressObjectives()
        {
            return inProgressObjectives;
        }

        public List<string> GetCompletedObjectives()
        {
            return completedObjectives;
        }
    }
}
