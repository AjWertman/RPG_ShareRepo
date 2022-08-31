using UnityEngine;
using RPGProject.UI;

namespace RPGProject.Inventories
{
    /// <summary>
    /// The UI to represent a slot in the inventory. If the slot is not empty, 
    /// it will contain the data of an inventory item.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;

        InventoryItem item;
        Inventory inventory;

        int index;

        public void InitializeSlotUI(Inventory _inventory)
        {
            inventory = _inventory;
        }

        public void Setup(int _index)
        {
            index = _index;
            icon.SetItem(inventory.GetItemInSlot(_index), inventory.GetNumberInSlot(_index));
        }

        public void AddItems(InventoryItem _item, int _number)
        {
            inventory.AddItemToSlot(index, _item, _number);
        }

        public void RemoveItems(int number)
        {
            if (inventory.GetItemInSlot(index) == null) return;
            inventory.RemoveFromSlot(index, number);
        }

        public void ResetSlot()
        {
            index = 0;
            icon.SetItem(null);
        }

        public int MaxAcceptable(InventoryItem _item)
        {
            if (inventory.HasSpaceFor(_item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetNumber()
        {
            return inventory.GetNumberInSlot(index);
        }
    }
}