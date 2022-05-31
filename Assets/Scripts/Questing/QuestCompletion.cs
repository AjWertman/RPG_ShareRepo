using UnityEngine;

namespace RPGProject.Questing
{
    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest = null;
        [SerializeField] string objectiveRefToComplete = null;

        public void CompleteObjective(PlayerQuestList _playerQuestList)
        {          
            QuestStatus status = _playerQuestList.GetQuestStatus(quest);

            if (quest.HasObjective(objectiveRefToComplete))
            {
                _playerQuestList.CompleteObjective(quest, objectiveRefToComplete);
            }
        }
    }
}
