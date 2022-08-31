using RPGProject.Questing;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGProject.UI
{
    /// <summary>
    /// Displays all information regarding a player's quest list.
    /// </summary>
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] Button questsTab = null;
        [SerializeField] Button completedTab = null;
        [SerializeField] Button backButton = null;

        [SerializeField] GameObject questItemPrefab = null;

        [SerializeField] Transform questListContent = null;
        [SerializeField] QuestTooltipUI questTooltipUI = null;

        PlayerQuestList questList = null;

        List<QuestItemUI> questItemUIs = new List<QuestItemUI>();

        public event Action onBackButton;

        public void InitializeMenu()
        {
            questsTab.onClick.AddListener(() => ActivateQuestsPage(false));
            completedTab.onClick.AddListener(() => ActivateQuestsPage(true));

            backButton.onClick.AddListener(() => onBackButton());

            questList = FindObjectOfType<PlayerQuestList>(true);
            CreateQuestItemUIPool(questList.GetQuestStatuses().Count);

            questList.onListUpdate += Redraw;
            questList.onQuestComplete += Redraw;

            DeactivateTooltip();
            SetSelectedTabColor(null);
            Redraw();
        }

        public void OnDisable()
        {
            ResetQuestItemUIs();
            DeactivateTooltip();
        }

        private void CreateQuestItemUIPool(int _questListCount)
        {
            int questCountSafetyNet = _questListCount + 10;

            for (int i = 0; i < questCountSafetyNet; i++)
            {
                GameObject questUIInstance = Instantiate(questItemPrefab, questListContent);
                QuestItemUI questItemUI = questUIInstance.GetComponent<QuestItemUI>();

                questItemUI.onMouseEnter += SetupTooltip;
                questItemUI.onMouseExit += DeactivateTooltip;

                questItemUIs.Add(questItemUI);
            }

            ResetQuestItemUIs();
        }

        public void ActivateQuestsPage(bool _completedPage)
        {
            if (_completedPage) SetSelectedTabColor(questsTab);
            else SetSelectedTabColor(completedTab);

            Redraw();
        }

        private void Redraw()
        {
            ResetQuestItemUIs();

            foreach (QuestStatus status in questList.GetQuestStatuses())
            {
                QuestItemUI questUIInstance = GetAvailableQuestItemUI();
                questUIInstance.SetupQuestItemUI(status);
               
                questUIInstance.transform.parent = questListContent;
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
            //completedTooltipUI.SetupTooltip(_status);
        }

        private void DeactivateTooltip()
        {
            questTooltipUI.DeactivateTooltip();
            //completedTooltipUI.DeactivateTooltip();
        }

        private void ResetQuestItemUIs()
        {
            foreach (QuestItemUI questItemUI in questItemUIs)
            {
                questItemUI.ResetQuestItemUI();
                questItemUI.gameObject.SetActive(false);
            }
        }

        private void SetSelectedTabColor(Button _selectedTab)
        {
            foreach (Button tab in GetTabs())
            {
                tab.interactable = true;
            }

            if (_selectedTab == null) return;
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
