using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPGProject.Inventories
{
    /// <summary>
    /// The image that represents an inventory item and the amount of that item.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] GameObject textContainer = null;
        [SerializeField] TextMeshProUGUI itemNumber = null;

        Image iconImage = null;

        private void Awake()
        {
            iconImage = GetComponent<Image>();
        }

        public void SetItem(InventoryItem _item)
        {
            SetItem(_item, 0);
        }

        public void SetItem(InventoryItem _item, int _number)
        {
            if (_item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = _item.icon;
            }

            if (_number <= 1)
            {
                textContainer.SetActive(false);
            }
            else
            {
                textContainer.SetActive(true);
                itemNumber.text = _number.ToString();
            }
        }
    }
}