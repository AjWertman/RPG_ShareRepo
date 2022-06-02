using System;
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

        Dictionary<QuestObjectiveUIItem, bool> objectiveObjects = new Dictionary<QuestObjectiveUIItem, bool>();

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
                QuestObjectiveUIItem questObjectiveUIItem = null;

                if (objective.GetRequiredObjectives().Length <= 0)
                {
                    questObjectiveUIItem = GetAvailableObjectiveInstance(true);
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
                        questObjectiveUIItem = GetAvailableObjectiveInstance(false);
                    }
                }

                questObjectiveUIItem.SetupObjectiveUIITem(_status, objective);
            }
        }

        private void CreateObjectiveObjects()
        {
            for (int i = 0; i < 4; i++)
            {
                QuestObjectiveUIItem objective = InstantiateObjectiveUIItem(objectivePrefab);
                objectiveObjects.Add(objective, false);
            }

            for (int i = 0; i < 4; i++)
            {
                QuestObjectiveUIItem objective = InstantiateObjectiveUIItem(objectivePrefab);
                objectiveObjects.Add(objective, true);
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
            foreach (QuestObjectiveUIItem objective in objectiveObjects.Keys)
            {
                objective.gameObject.SetActive(false);
            }
        }

        public void DeactivateTooltip()
        {
            questSelectedObject.SetActive(false);
            noQuestSelectedObject.SetActive(true);
        }

        private QuestObjectiveUIItem GetAvailableObjectiveInstance(bool _completedObjectiveObject)
        {
            QuestObjectiveUIItem availableObjectiveInstance = null;

            foreach (QuestObjectiveUIItem objectiveObject in objectiveObjects.Keys)
            {
                if (objectiveObject.gameObject.activeSelf) continue;
                if(objectiveObjects[objectiveObject] == _completedObjectiveObject)
                {
                    availableObjectiveInstance = objectiveObject;
                    break;
                }
            }

            return availableObjectiveInstance;
        }
    }
}