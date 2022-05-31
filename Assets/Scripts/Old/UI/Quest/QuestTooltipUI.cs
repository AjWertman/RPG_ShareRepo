using RPGProject.Questing;
using TMPro;
using UnityEngine;

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

    public void Setup(QuestStatus _status)
    {
        Quest quest = _status.GetQuest();
        title.text = quest.name;
        description.text = quest.GetDescription();

        foreach(Transform item in objectiveContainer.transform)
        {
            Destroy(item.gameObject);
        }

        foreach(var objective in quest.GetObjectives())
        {
            if (objective.GetRequiredObjectives().Length <= 0)
            {
                CreateObjectiveUIItem(_status, objective);
            }
            else
            {
                bool hasAllRequiredObjectives = true;

                foreach(string requiredObjective in objective.GetRequiredObjectives())
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
                    CreateObjectiveUIItem(_status, objective);
                }
            }          
        }

        noQuestSelectedObject.SetActive(false);
        questSelectedObject.SetActive(true);
    }

    private void CreateObjectiveUIItem(QuestStatus _status, Objective _objective)
    {
        GameObject prefab = objectivePrefab;

        if (_status.IsObjectiveComplete(_objective.GetReference()))
        {
            prefab = completeObjectivePrefab;
        }

        GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);

        objectiveInstance.GetComponent<QuestObjectiveUIItem>().SetupObjectiveUIITem(_status, _objective);
    }

    public void ClearTooltip()
    {
        questSelectedObject.SetActive(false);
        noQuestSelectedObject.SetActive(true);
    }
}
