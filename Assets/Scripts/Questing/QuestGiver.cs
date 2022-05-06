using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] Quest questToGive = null;

    public void GiveQuest()
    {
        PlayerQuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuestList>();

        questList.AddQuest(questToGive);
    }
}
