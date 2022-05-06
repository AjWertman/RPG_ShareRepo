using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerQuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
{
    List<QuestStatus> questStatuses = new List<QuestStatus>();
    List<QuestStatus> completedQuestStatuses = new List<QuestStatus>();

    public event Action onListUpdate;
    public event Action onQuestComplete;

    public void AddQuest(Quest quest)
    {
        if (HasQuest(quest)) return;

        QuestStatus newStatus = new QuestStatus(quest);
        questStatuses.Add(newStatus);

        onListUpdate();
    }

    public void CompleteObjective(Quest quest, string objectiveToComplete)
    {
        QuestStatus status = GetQuestStatus(quest);

        if (status == null) return;

        status.CompleteObjective(objectiveToComplete);

        if (status.IsComplete())
        {
            GiveReward(quest);
        }

        onListUpdate();
    }

    public bool HasQuest(Quest quest)
    {
        return GetQuestStatus(quest) != null;
    }

    public IEnumerable<QuestStatus> GetQuestStatueses()
    {
        return questStatuses;
    }

    public QuestStatus GetQuestStatus(Quest quest)
    {
        foreach (QuestStatus status in questStatuses)
        {          
            if (status.GetQuest() == quest)
            {
                return status;
            }
        }

        return null;
    }

    private void GiveReward(Quest quest)
    {
        foreach(var reward in quest.GetRewards())
        {
            //bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.number);
            //if (!success)
            //{
            //    GetComponent<ItemDropper>().DropItem(reward.item, reward.number);
            //}
        }

        FindObjectOfType<PlayerTeam>().AwardTeamXP(quest.GetXPAward());
    }

    public object CaptureState()
    {
        List<object> state = new List<object>();

        foreach(QuestStatus status in questStatuses)
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
        foreach(object objectState in stateList)
        {
            questStatuses.Add(new QuestStatus(objectState));          
        }

        if(questStatuses.Count > 0)
        {
            onListUpdate();
        }
    }

    public bool? Evaluate(string predicate, string[] parameters)
    {
        switch (predicate)
        {
            case "HasQuest":
                return HasQuest(Quest.GetByName(parameters[0]));

            case "CompletedQuest":
                QuestStatus status = GetQuestStatus(Quest.GetByName(parameters[0]));
                if (status == null) return false;
                return status.IsComplete();       
        }

        return null;
    }
}
