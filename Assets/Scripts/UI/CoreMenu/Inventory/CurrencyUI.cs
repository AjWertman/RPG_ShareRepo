using TMPro;
using UnityEngine;

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
        currencyText.text = ("Leons: " + currency.GetGold().ToString());
    }
}
