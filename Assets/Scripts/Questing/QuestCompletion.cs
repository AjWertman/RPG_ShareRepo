using UnityEngine;

namespace RPGProject.Questing
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] GameObject quest_ionMark = null;
        [SerializeField] Quest quest = null;
        [SerializeField] string objectiveRefToComplete = null;

        PlayerQuestList playerQuestList = null;
        QuestStatus status = null;

        private void Awake()
        {
            playerQuestList = FindObjectOfType<PlayerQuestList>();

            status = playerQuestList.GetQuestStatus(quest);

            IsQuestComplete();
        }

        public void CompleteObjective()
        {          
            if (quest.HasObjective(objectiveRefToComplete))
            {
                playerQuestList.CompleteObjective(quest, objectiveRefToComplete);
            }
        }

        private bool IsQuestComplete()
        {
            bool isQuestComplete = status.IsComplete();

            quest_ionMark.SetActive(isQuestComplete);

            return isQuestComplete;
        }
    }
}
