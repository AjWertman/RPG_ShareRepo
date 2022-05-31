using UnityEngine;
using UnityEngine.UI;

public class UIHealthChange : MonoBehaviour
{
    [SerializeField] GameObject criticalText = null;
    [SerializeField] GameObject amountChangeText = null;

    public void ActivateCriticalCanvas(bool shouldActivate, bool isDamage)
    {
        if (isDamage)
        {
            criticalText.GetComponent<Text>().color = Color.red;
        }
        else
        {
            criticalText.GetComponent<Text>().color = Color.green;
        }

        criticalText.SetActive(shouldActivate);
    }

    public void ActivateAmountCanvas(bool shouldActivate, bool isDamage, float amount)
    {
        amountChangeText.SetActive(shouldActivate);

        if (shouldActivate)
        {
            if (isDamage)
            {
                criticalText.GetComponent<Text>().color = Color.red;
                amountChangeText.GetComponent<Text>().color = Color.red;
                amountChangeText.GetComponent<Text>().text = (amount.ToString());
            }
            else
            {
                criticalText.GetComponent<Text>().color = Color.green;
                amountChangeText.GetComponent<Text>().color = Color.green;
                amountChangeText.GetComponent<Text>().text = (amount.ToString());
            }
        }
        else
        {
            amountChangeText.GetComponent<Text>().text = "";
        }

    }
}
