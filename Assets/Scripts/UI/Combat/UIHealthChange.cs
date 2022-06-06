using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
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

            if (!_isDamage)
            {
                criticalText.color = Color.green;
            }

            amountChangeText.text = _changeAmount.ToString();
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
