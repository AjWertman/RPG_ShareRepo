using UnityEngine;

namespace RPGProject.Questing
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] string objectiveRefToComplete = null;

        public Quest quest = null;

        PlayerQuestList playerQuestList = null;
        QuestStatus status = null;

        private void Start()
        {
            playerQuestList = FindObjectOfType<PlayerQuestList>();
        }

        public void CompleteObjective()
        {
            if (!playerQuestList.HasQuest(quest)) return;
            if (quest.HasObjective(objectiveRefToComplete))
            {
                playerQuestList.CompleteObjective(quest, objectiveRefToComplete);
            }
        }
    }
}
