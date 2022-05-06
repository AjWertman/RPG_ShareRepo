using UnityEngine;

public class QuestCompletion : MonoBehaviour
{
    [SerializeField] Quest quest = null;
    [SerializeField] string objectiveRefToComplete = null;

    public void CompleteObjective()
    {
        PlayerQuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuestList>();
        QuestStatus status = questList.GetQuestStatus(quest);

        if (quest.HasObjective(objectiveRefToComplete))
        {
            questList.CompleteObjective(quest, objectiveRefToComplete);
        }
    } 
}
