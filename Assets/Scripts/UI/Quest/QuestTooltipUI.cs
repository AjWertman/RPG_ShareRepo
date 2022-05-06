using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void Setup(QuestStatus status)
    {
        Quest quest = status.GetQuest();
        title.text = quest.name;
        description.text = quest.GetDescription();

        foreach(Transform item in objectiveContainer.transform)
        {
            Destroy(item.gameObject);
        }

        foreach(var objective in quest.GetObjectives())
        {
            if (objective.requiredObjectives.Length <= 0)
            {
                CreateObjectiveUIItem(status, objective);
            }
            else
            {
                bool hasAllRequiredObjectives = true;

                foreach(string requiredObjective in objective.requiredObjectives)
                {
                    if (status.IsObjectiveComplete(requiredObjective)) continue;
                    else
                    {
                        hasAllRequiredObjectives = false;
                        break;
                    }
                }

                if (hasAllRequiredObjectives)
                {
                    CreateObjectiveUIItem(status, objective);
                }
            }          
        }

        noQuestSelectedObject.SetActive(false);
        questSelectedObject.SetActive(true);
    }

    private void CreateObjectiveUIItem(QuestStatus status, Quest.Objective objective)
    {
        GameObject prefab = objectivePrefab;

        if (status.IsObjectiveComplete(objective.reference))
        {
            prefab = completeObjectivePrefab;
        }

        GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);

        objectiveInstance.GetComponent<QuestObjectiveUIItem>().SetupObjectiveUIITem(status, objective);
    }

    public void ClearTooltip()
    {
        questSelectedObject.SetActive(false);
        noQuestSelectedObject.SetActive(true);
    }
}
