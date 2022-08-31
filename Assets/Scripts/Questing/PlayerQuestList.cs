using RPGProject.Core;
using RPGProject.Saving;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Questing
{
    /// <summary>
    /// The collection of quests and their statuses that a player has.
    /// </summary>
    public class PlayerQuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> questStatuses = new List<QuestStatus>();
        List<QuestStatus> completedQuestStatuses = new List<QuestStatus>();

        public event Action onListUpdate;
        public event Action onQuestComplete;

        public event Action<float> onAward;

        public void AddQuest(Quest _quest)
        {
            if (HasQuest(_quest)) return;

            QuestStatus newStatus = new QuestStatus(_quest);
            questStatuses.Add(newStatus);

            onListUpdate();
        }

        public void CompleteObjective(Quest _quest, string _objectiveToComplete)
        {
            QuestStatus status = GetQuestStatus(_quest);

            if (status == null)
            {
                return;
            }

            status.CompleteObjective(_objectiveToComplete);


            if (status.IsComplete())
            {
                GiveReward(_quest);
            }

            onListUpdate();
        }

        public bool HasQuest(Quest _quest)
        {
            return GetQuestStatus(_quest) != null;
        }

        public List<QuestStatus> GetQuestStatuses()
        {
            return questStatuses;
        }

        public QuestStatus GetQuestStatus(Quest _quest)
        {

            foreach (QuestStatus status in questStatuses)
            {
                if (status.quest == _quest)
                {
                    return status;
                }
            }

            return null;
        }

        private void GiveReward(Quest _quest)
        {
            foreach (var reward in _quest.rewards)
            {
                //bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
                //if (!success)
                //{
                //    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
                //}
            }

            onAward(_quest.xpAward);
            //playerTeam.AwardTeamXP(_quest.GetXPAward());
        }

        public object CaptureState()
        {
            List<object> state = new List<object>();

            foreach (QuestStatus status in questStatuses)
            {
                state.Add(status.CaptureState());
            }
            return state;
        }

        public void RestoreState(object state)
        {
            List<object> stateList = state as List<object>;
            if (stateList == null) return;

            questStatuses.Clear();
            foreach (object objectState in stateList)
            {
                questStatuses.Add(new QuestStatus(objectState));
            }

            if (questStatuses.Count > 0)
            {
                onListUpdate();
            }
        }

        public bool? Evaluate(string _predicate, string[] _parameters)
        {
            switch (_predicate)
            {
                case "HasQuest":
                    Quest quest = Quest.GetByName(_parameters[0]);
                    return HasQuest(quest);

                case "CompletedQuest":
                    QuestStatus status = GetQuestStatus(Quest.GetByName(_parameters[0]));
                    if (status == null) return false;
                    return status.IsComplete();
            }

            return null;
        }
    }
}