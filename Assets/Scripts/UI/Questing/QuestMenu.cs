using RPGProject.Questing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] Button questsTab = null;
        [SerializeField] Button completedTab = null;
        [SerializeField] GameObject questItemPrefab = null;

        [Header("Quests")]
        [SerializeField] GameObject questsPage = null;
        [SerializeField] Transform questListContent = null;
        [SerializeField] QuestTooltipUI questTooltipUI = null;

        [Header("Completed")]
        [SerializeField] GameObject completedQuestsPage = null;
        [SerializeField] Transform completedListContent = null;
        [SerializeField] QuestTooltipUI completedTooltipUI = null;

        PlayerQuestList questList = null;

        List<QuestItemUI> questItemUIs = new List<QuestItemUI>();

        private void Start()
        {
            questsTab.onClick.AddListener(() => ActivateQuestsPage());
            completedTab.onClick.AddListener(() => ActivateCompletedPage());

            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerQuestList>();
            CreateQuestItemUIPool(questList.GetQuestStatuses().Count);

            questList.onListUpdate += Redraw;
            questList.onQuestComplete += Redraw;

            DeactivateTooltips();
            Redraw();
        }

        public void OnDisable()
        {
            ResetQuestItemUIs();
            DeactivateTooltips();
        }

        private void CreateQuestItemUIPool(int _questListCount)
        {
            int questCountSafetyNet = _questListCount + 10;

            for (int i = 0; i < questCountSafetyNet; i++)
            {
                GameObject questUIInstance = Instantiate(questItemPrefab, questListContent);
                QuestItemUI questItemUI = questUIInstance.GetComponent<QuestItemUI>();

                questItemUI.onMouseEnter += SetupTooltip;
                questItemUI.onMouseExit += DeactivateTooltips;

                questItemUIs.Add(questItemUI);
            }

            ResetQuestItemUIs();
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
            ResetQuestItemUIs();

            foreach (QuestStatus status in questList.GetQuestStatuses())
            {
                QuestItemUI questUIInstance = GetAvailableQuestItemUI();
                questUIInstance.SetupQuestItemUI(status);

                Transform newParent = null;

                if (!status.IsComplete())
                {
                    newParent = questListContent;
                }
                else
                {
                    newParent = completedListContent;
                }

                questUIInstance.transform.parent = newParent;
                questUIInstance.gameObject.SetActive(true);
            }
        }

        private void MoveQuestToCompleted()
        {
            Redraw();
        }

        private void SetupTooltip(QuestStatus _status)
        {
            questTooltipUI.SetupTooltip(_status);
            completedTooltipUI.SetupTooltip(_status);
        }

        private void DeactivateTooltips()
        {
            questTooltipUI.DeactivateTooltip();
            completedTooltipUI.DeactivateTooltip();
        }

        private void ResetQuestItemUIs()
        {
            foreach (QuestItemUI questItemUI in questItemUIs)
            {
                questItemUI.ResetQuestItemUI();
                questItemUI.transform.parent = transform;
                questItemUI.gameObject.SetActive(false);
            }
        }

        private void SetSelectedTabColor(Button _selectedTab)
        {
            foreach (Button tab in GetTabs())
            {
                tab.interactable = true;
            }
            _selectedTab.interactable = false;
        }

        private QuestItemUI GetAvailableQuestItemUI()
        {
            QuestItemUI availableQuestItemUI = null;

            foreach (QuestItemUI questItemUI in questItemUIs)
            {
                if (questItemUI.GetQuestStatus() == null)
                {
                    availableQuestItemUI = questItemUI;
                    break;
                }
            }

            //if(isnull) createnew

            return availableQuestItemUI;
        }

        private IEnumerable<Button> GetTabs()
        {
            yield return questsTab;
            yield return completedTab;
        }
    }
}
