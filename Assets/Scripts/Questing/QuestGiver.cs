using UnityEngine;

namespace RPGProject.Questing
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] GameObject exclamationPointObject = null;
        [SerializeField] Quest questToGive = null;

        private void Awake()
        {
            if (HasQuest())
            {
                exclamationPointObject.SetActive(true);
            }
        }

        public void GiveQuest(PlayerQuestList _playerQuestList)
        {
            _playerQuestList.AddQuest(questToGive);
            exclamationPointObject.SetActive(false);
        }

        public bool HasQuest()
        {
            return (questToGive != null);
        }
    }
}