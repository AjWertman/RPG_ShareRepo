using TMPro;
using UnityEngine;

public class QuestObjectiveUIItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText = null;
    [SerializeField] TextMeshProUGUI progressText = null;

    public void SetupObjectiveUIITem(QuestStatus status, Quest.Objective objective)
    {
        int amountCompleted = 0;
        int amountToComplete = 0;

        amountCompleted = status.GetInProgressObjectives()[objective.reference];
        amountToComplete = objective.amountToComplete;

        string progressString = amountCompleted.ToString() + "/" + amountToComplete.ToString();

        descriptionText.text = objective.description;
        progressText.text = progressString;
    }
}
