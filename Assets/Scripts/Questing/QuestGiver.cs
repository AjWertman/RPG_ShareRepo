using UnityEngine;

namespace RPGProject.Questing
{
    /// <summary>
    /// Placed on objects that can give a player quests.
    /// </summary>
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] GameObject exclamationPointObject = null;
        [SerializeField] Quest questToGive = null;

        PlayerQuestList playerQuestList = null;

        private void Awake()
        {
            playerQuestList = FindObjectOfType<PlayerQuestList>();
            HasQuest();
        }

        public void GiveQuest()
        {
            playerQuestList.AddQuest(questToGive);
            questToGive = null;

            HasQuest();
        }

        public bool HasQuest()
        {
            bool hasQuest = questToGive != null;
            exclamationPointObject.SetActive(hasQuest);

            return hasQuest;
        }
    }
}