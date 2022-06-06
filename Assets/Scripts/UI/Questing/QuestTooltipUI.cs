﻿using System;
using System.Collections.Generic;
using RPGProject.Questing;
using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] GameObject questSelectedObject = null;
        [SerializeField] GameObject noQuestSelectedObject = null;

        [SerializeField] TextMeshProUGUI title = null;
        [SerializeField] TextMeshProUGUI description = null;

        [SerializeField] Transform objectiveContainer = null;
        [SerializeField] GameObject objectivePrefab = null;
        [SerializeField] GameObject completeObjectivePrefab = null;

        [SerializeField] Transform rewardContainer = null;
        [SerializeField] GameObject rewardPrefab = null;

        List<QuestObjectiveUIItem> objectiveObjects = new List<QuestObjectiveUIItem>();

        private void Awake()
        {
            CreateObjectiveObjects();
        }

        public void SetupTooltip(QuestStatus _status)
        {
            Quest quest = _status.GetQuest();
            title.text = quest.name;
            description.text = quest.GetDescription();

            PopulateObjectives(_status);
         
            noQuestSelectedObject.SetActive(false);
            questSelectedObject.SetActive(true);
        }

        private void PopulateObjectives(QuestStatus _status)
        {           
            DeactivateObjectiveObjects();

            Quest quest = _status.GetQuest();

            foreach (Objective objective in quest.GetObjectives())
            { 
                QuestObjectiveUIItem questObjectiveUIItem = GetAvailableObjectiveInstance();
                bool isCompleted = false;

                if (objective.GetRequiredObjectives().Length <= 0)
                {
                    isCompleted = true;
                    questObjectiveUIItem = GetAvailableObjectiveInstance();
                }
                else
                {
                    bool hasAllRequiredObjectives = true;

                    foreach (string requiredObjective in objective.GetRequiredObjectives())
                    {
                        if (_status.IsObjectiveComplete(requiredObjective)) continue;
                        else
                        {
                            hasAllRequiredObjectives = false;
                            break;
                        }
                    }

                    if (hasAllRequiredObjectives)
                    {
                        isCompleted = false;                        
                    }
                }

                questObjectiveUIItem.SetupObjectiveUIITem(_status, objective);
                questObjectiveUIItem.gameObject.SetActive(true);
            }
        }

        private void CreateObjectiveObjects()
        {
            for (int i = 0; i < 5; i++)
            {
                QuestObjectiveUIItem objective = InstantiateObjectiveUIItem(objectivePrefab);
                objectiveObjects.Add(objective);
            }

            DeactivateObjectiveObjects();
        }

        private QuestObjectiveUIItem InstantiateObjectiveUIItem(GameObject _prefab)
        {
            GameObject objectiveInstance = Instantiate(_prefab, objectiveContainer);
            QuestObjectiveUIItem questObjectiveUIItem = objectiveInstance.GetComponent<QuestObjectiveUIItem>();
            questObjectiveUIItem.ResetUIItem();

            return questObjectiveUIItem;
        }

        private void DeactivateObjectiveObjects()
        {
            foreach (QuestObjectiveUIItem objective in objectiveObjects)
            {
                objective.gameObject.SetActive(false);
            }
        }

        public void DeactivateTooltip()
        {
            questSelectedObject.SetActive(false);
            noQuestSelectedObject.SetActive(true);
        }

        private QuestObjectiveUIItem GetAvailableObjectiveInstance()
        {
            QuestObjectiveUIItem availableObjectiveInstance = null;

            foreach (QuestObjectiveUIItem objectiveObject in objectiveObjects)
            {
                if(objectiveObject.gameObject.activeSelf == false)
                {
                    availableObjectiveInstance = objectiveObject;
                    break;
                }
            }

            return availableObjectiveInstance;
        }
    }
}