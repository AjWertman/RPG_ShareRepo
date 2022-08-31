using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
    /// <summary>
    /// Indicates the amount of damage or healing done by an ability, 
    /// also indicating if it was a critical hit.
    /// </summary>
    public class UIHealthChange : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI criticalText = null;
        [SerializeField] TextMeshProUGUI amountChangeText = null;

        private void Awake()
        {
            DeactivateHealthChange();
            criticalText.text = "Critical!";
        }

        public void ActivateHealthChange(bool _isCritical, float _changeAmount)
        {
            bool isDamage = _changeAmount < 0;

            ActivateCriticalCanvas(_isCritical, isDamage);
            ActivateAmountCanvas(_changeAmount, isDamage);
        }
      
        private void ActivateCriticalCanvas(bool _isCritical, bool _isDamage)
        {
            if (!_isCritical)
            {
                DeactivateCriticalCanvas();
                return;
            }

            criticalText.color = Color.red;

            if (!_isDamage)
            {
                criticalText.color = Color.green;
            }

            criticalText.gameObject.SetActive(true);
        }

        private void ActivateAmountCanvas(float _changeAmount, bool _isDamage)
        {
            amountChangeText.color = Color.red;

            string beforeChangeAmountString = "";

            if (!_isDamage)
            {
                amountChangeText.color = Color.green;
                beforeChangeAmountString = "+";

            }

            string text = (beforeChangeAmountString + _changeAmount.ToString());

            amountChangeText.text = text;
            amountChangeText.gameObject.SetActive(true);
        }

        public void DeactivateHealthChange()
        {
            DeactivateCriticalCanvas();
            DeactivateAmountCanvas();
        }

        private void DeactivateCriticalCanvas()
        {
            criticalText.color = Color.black;
            criticalText.gameObject.SetActive(false);
        }

        private void DeactivateAmountCanvas()
        {
            amountChangeText.color = Color.black;
            amountChangeText.text = "";
            amountChangeText.gameObject.SetActive(false);
        }
    }
}
