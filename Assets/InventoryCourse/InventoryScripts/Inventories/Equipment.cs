using System;
using System.Collections.Generic;
using UnityEngine;
using RPGProject.Saving;

namespace RPGProject.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();

        public event Action equipmentUpdated;

        public void AddItem(EquipLocation _slot, EquipableItem _item)
        {
            Debug.Assert(_item.GetAllowedEquipLocation() == _slot);

            equippedItems[_slot] = _item;

            equipmentUpdated();
        }

        public void RemoveItem(EquipLocation _slot)
        {
            equippedItems.Remove(_slot);

            equipmentUpdated();
        }

        public EquipableItem GetItemInSlot(EquipLocation _equipLocation)
        {
            if (!equippedItems.ContainsKey(_equipLocation))
            {
                return null;
            }

            return equippedItems[_equipLocation];
        }

        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        public object CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }

        public void RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
                {
                    equippedItems[pair.Key] = item;
                }
            }
        }
    }
}