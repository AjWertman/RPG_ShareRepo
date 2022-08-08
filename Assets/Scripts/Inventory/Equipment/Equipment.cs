using System;
using System.Collections.Generic;
using UnityEngine;
using RPGProject.Saving;

namespace RPGProject.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        Dictionary<EquipmentSlot, EquipableItem> equippedItems = new Dictionary<EquipmentSlot, EquipableItem>();

        public event Action equipmentUpdated;

        private void Awake()
        {
            InitalizeEquipment();
        }

        public void AddItem(EquipmentSlot _equipmentSlot, EquipableItem _item)
        {
            equippedItems[_equipmentSlot] = _item;

            equipmentUpdated();
        }

        public void RemoveItem(EquipmentSlot _equipmentSlot)
        {
            equippedItems[_equipmentSlot] = null;

            equipmentUpdated();
        }

        private void InitalizeEquipment()
        {
            EquipmentSlot[] enumValues = (EquipmentSlot[])(Enum.GetValues(typeof(EquipmentSlot)));

            foreach (EquipmentSlot equipmentSlot in enumValues)
            {
                equippedItems.Add(equipmentSlot, null);
            }
        }

        public EquipableItem GetItemInSlot(EquipmentSlot _equipmentSlot)
        {
            if (!equippedItems.ContainsKey(_equipmentSlot)) return null;
            return equippedItems[_equipmentSlot];
        }

        public IEnumerable<EquipmentSlot> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        public object CaptureState()
        {
            Dictionary<EquipmentSlot,string> equippedItemsForSerialization = new Dictionary<EquipmentSlot, string>();
            foreach (EquipmentSlot equipmentSlot in equippedItems.Keys)
            {
                EquipableItem equippedItem = equippedItems[equipmentSlot];
                string itemID = "";

                if (equippedItem != null) itemID = equippedItem.itemID;

                equippedItemsForSerialization[equipmentSlot] = itemID;
            }
            return equippedItemsForSerialization;
        }

        public void RestoreState(object state)
        {
            Dictionary<EquipmentSlot, string> equippedItemsForSerialization = (Dictionary<EquipmentSlot, string>)state;
            foreach (EquipmentSlot equipmentSlot in equippedItemsForSerialization.Keys)
            {
                EquipableItem item = (EquipableItem)InventoryItem.GetFromID(equippedItemsForSerialization[equipmentSlot]);
                equippedItems[equipmentSlot] = item;
            }
        }
    }
}