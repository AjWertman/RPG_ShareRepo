using UnityEngine;

namespace RPGProject.Questing
{
    /// <summary>
    /// Placed on objects that can complete objectives for the player.
    /// </summary>
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
