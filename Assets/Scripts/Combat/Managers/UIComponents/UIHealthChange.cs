using TMPro;
using UnityEngine;

namespace RPGProject.Combat
{
    public class UIHealthChange : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI criticalText = null;
        [SerializeField] TextMeshProUGUI amountChangeText = null;

        public void ActivateCriticalCanvas(bool _shouldActivate, bool _isDamage)
        {
            if (_isDamage)
            {
                criticalText.color = Color.red;
            }
            else
            {
                criticalText.color = Color.green;
            }

            criticalText.gameObject.SetActive(_shouldActivate);
        }

        public void ActivateAmountCanvas(bool _shouldActivate, bool _isDamage, float _amount)
        {
            if (_shouldActivate)
            {
                Color textColor = Color.black;
                if (_isDamage)
                {
                    textColor = Color.red;
                }
                else
                {
                    textColor = Color.green;
                }

                amountChangeText.color = textColor;
                amountChangeText.text = (_amount.ToString());
            }
            else
            {
                amountChangeText.text = "";
            }
            amountChangeText.gameObject.SetActive(_shouldActivate);
        }
    }
}
