using RPGProject.Questing;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGProject.UI
{
    public class QuestItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] TextMeshProUGUI title = null;

        QuestStatus currentQuestStatus = null;

        public event Action<QuestStatus> onMouseEnter;
        public event Action onMouseExit;

        public void SetupQuestItemUI(QuestStatus _status)
        {
            currentQuestStatus = _status;
            title.text = currentQuestStatus.quest.GetTitle();
        }

        public void ResetQuestItemUI()
        {
            currentQuestStatus = null;
            title.text = "";
        }

        public QuestStatus GetQuestStatus()
        {
            return currentQuestStatus;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onMouseEnter(currentQuestStatus);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onMouseExit();
        }
    }
}