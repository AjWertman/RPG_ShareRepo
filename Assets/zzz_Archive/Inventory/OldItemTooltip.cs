using UnityEngine;
using TMPro;
using RPGProject.Inventories;

public class OldItemTooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText = null;
    [SerializeField] TextMeshProUGUI bodyText = null;

    public void Setup(InventoryItem _item)
    {
        titleText.text = _item.displayName;
        bodyText.text = _item.description;
    }
}