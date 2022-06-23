using UnityEngine;
using TMPro;

namespace RPGProject.Inventories
{
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;

        public void Setup(InventoryItem _item)
        {
            titleText.text = _item.GetDisplayName();
            bodyText.text = _item.GetDescription();
        }
    }
}
