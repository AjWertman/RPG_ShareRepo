using RPGProject.Inventories;
using TMPro;
using UnityEngine;

namespace RPGProject.UI
{
    /// <summary>
    /// Used to represent the current amount of currency the player has.
    /// </summary>
    public class CurrencyUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI currencyText = null;

        Currency currency = null;

        private void Awake()
        {
            currency = GameObject.FindGameObjectWithTag("Player").GetComponent<Currency>();
            currency.onCurrencyChange += UpdateCurrencyText;
        }

        private void Start()
        {
            UpdateCurrencyText();
        }

        public void UpdateCurrencyText()
        {
            currencyText.text = ("Gold: " + currency.gold.ToString());
        }
    }
}