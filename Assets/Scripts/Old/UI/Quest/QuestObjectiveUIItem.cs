using RPGProject.Questing;
using TMPro;
using UnityEngine;

public class QuestObjectiveUIItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI descriptionText = null;
    [SerializeField] TextMeshProUGUI progressText = null;

    public void SetupObjectiveUIITem(QuestStatus _status, Objective _objective)
    {
        int amountCompleted = 0;
        int amountToComplete = 0;

        amountCompleted = _status.GetInProgressObjectives()[_objective.GetReference()];
        amountToComplete = _objective.GetAmountToComplete();

        string progressString = amountCompleted.ToString() + "/" + amountToComplete.ToString();

        descriptionText.text = _objective.GetDescription();
        progressText.text = progressString;
    }
}
