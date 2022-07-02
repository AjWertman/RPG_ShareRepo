using UnityEngine;
using TMPro;
using RPGProject.Inventories;

public class OldItemTooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI bodyText = null;

    public void Setup(InventoryItem _item)
    {
        titleText.text = _item.GetDisplayName();
        bodyText.text = _item.GetDescription();
    }
}