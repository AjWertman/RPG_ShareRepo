using RPGProject.Questing;
using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
    public class QuestObjectiveUIItem : MonoBehaviour
    {
        [SerializeField] GameObject nonCompletedImage = null;
        [SerializeField] GameObject completedImage = null;

        [SerializeField] TextMeshProUGUI descriptionText = null;
        [SerializeField] TextMeshProUGUI progressText = null;

        public void SetupObjectiveUIITem(QuestStatus _status, Objective _objective)
        {
            int amountCompleted = 0;
            int amountToComplete = 0;

            amountCompleted = _status.GetInProgressObjectives()[_objective.GetReference()];
            amountToComplete = _objective.GetAmountToComplete();

            bool isCompleted = amountCompleted >= amountToComplete;
            SetUIImage(isCompleted);

            string progressString = amountCompleted.ToString() + "/" + amountToComplete.ToString();

            descriptionText.text = _objective.GetDescription();
            progressText.text = progressString;
        }

        public void ResetUIItem()
        {
            descriptionText.text = "";
            progressText.text = "";
        }

        private void SetUIImage(bool _isCompleted)
        {
            DeactivateImages();

            if (_isCompleted) completedImage.SetActive(true);
            else nonCompletedImage.SetActive(true);
        }

        private void DeactivateImages()
        {
            nonCompletedImage.SetActive(false);
            completedImage.SetActive(false);
        }
    }
}