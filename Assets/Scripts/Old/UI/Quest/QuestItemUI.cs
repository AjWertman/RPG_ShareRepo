using RPGProject.Questing;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI title = null;

    QuestStatus currentQuestStatus = null;

    public event Action<QuestStatus> onMouseEnter;
    public event Action onMouseExit;

    public void Setup(QuestStatus status)
    {
        currentQuestStatus = status;
        title.text = currentQuestStatus.GetQuest().name;
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
