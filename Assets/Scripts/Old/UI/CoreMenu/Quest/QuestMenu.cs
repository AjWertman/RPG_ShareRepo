using RPGProject.Questing;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : MonoBehaviour
{
    [SerializeField] Button questsTab = null;
    [SerializeField] Button completedTab = null;
    [SerializeField] QuestItemUI questItemPrefab = null;

    [Header("Quests")]
    [SerializeField] GameObject questsPage = null;    
    [SerializeField] Transform questListContent = null;
    [SerializeField] QuestTooltipUI questTooltipUI = null;

    [Header("Completed")]
    [SerializeField] GameObject completedQuestsPage = null;
    [SerializeField] Transform completedListContent = null;
    [SerializeField] QuestTooltipUI completedTooltipUI = null;

    PlayerQuestList questList = null;

    private void Start()
    {
        questsTab.onClick.AddListener(() => ActivateQuestsPage());
        completedTab.onClick.AddListener(() => ActivateCompletedPage());

        questList = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuestList>();
        questList.onListUpdate += Redraw;
        questList.onQuestComplete += Redraw;
    
        questTooltipUI.ClearTooltip();
        completedTooltipUI.ClearTooltip();
        Redraw();
    }

    public void OnDisable()
    {
        questTooltipUI.ClearTooltip();
        completedTooltipUI.ClearTooltip();
    }

    public void ActivateQuestsPage()
    {
        SetSelectedTabColor(questsTab);
        completedQuestsPage.SetActive(false);
        questsPage.SetActive(true);
    }

    private void ActivateCompletedPage()
    {
        SetSelectedTabColor(completedTab);
        questsPage.SetActive(false);
        completedQuestsPage.SetActive(true);
    }

    private void Redraw()
    {
        foreach (Transform childTransform in questListContent)
        {
            Destroy(childTransform.gameObject);
        }

        foreach(Transform childTransform in completedListContent)
        {
            Destroy(childTransform.gameObject);
        }

        foreach (QuestStatus status in questList.GetQuestStatueses())
        {
            QuestItemUI questUIInstance = null;

            if (!status.IsComplete())
            {
                questUIInstance = Instantiate<QuestItemUI>(questItemPrefab, questListContent);
            }
            else
            {
                questUIInstance = Instantiate<QuestItemUI>(questItemPrefab, completedListContent);            
            }

            questUIInstance.Setup(status);
            questUIInstance.onMouseEnter += SetupTooltip;
            questUIInstance.onMouseExit += ClearTooltip;
        }
    }

    private void MoveQuestToCompleted()
    {
        Redraw();
    }

    private void SetupTooltip(QuestStatus status)
    {
        questTooltipUI.Setup(status);
        completedTooltipUI.Setup(status);
    }

    private void ClearTooltip()
    {
        questTooltipUI.ClearTooltip();
        completedTooltipUI.ClearTooltip();
    }

    private void SetSelectedTabColor(Button selectedTab)
    {
        foreach (Button tab in GetTabs())
        {
            tab.interactable = true;
        }
        selectedTab.interactable = false;
    }

    private IEnumerable<Button> GetTabs()
    {
        yield return questsTab;
        yield return completedTab;
    }
}
