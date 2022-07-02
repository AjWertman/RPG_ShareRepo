using System;
using UnityEngine;
using RPGProject.Saving;

namespace RPGProject.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable
    {
        [SerializeField] int inventorySize = 16;

        InventorySlot[] slots;

        public event Action inventoryUpdated;

        private void Awake()
        {
            slots = new InventorySlot[inventorySize];

            //Testing
            InventoryItem armor = InventoryItem.GetFromID("0ce3da5d-7ed0-42ba-a84d-285fd7ad48ab");
            InventoryItem potion = InventoryItem.GetFromID("d48ab");

            AddToFirstEmptySlot(armor, 1);
            AddItemToSlot(8, potion, 6);
        }

        public bool AddItemToSlot(int _slot, InventoryItem _item, int _number)
        {
            if (slots[_slot].item != null)
            {
                return AddToFirstEmptySlot(_item, _number); ;
            }

            var i = FindStack(_item);
            if (i >= 0)
            {
                _slot = i;
            }

            slots[_slot].item = _item;
            slots[_slot].number += _number;

            inventoryUpdated();

            return true;
        }

        public bool AddToFirstEmptySlot(InventoryItem _item, int _number)
        {
            int i = FindSlot(_item);

            if (i < 0)
            {
                return false;
            }

            slots[i].item = _item;
            slots[i].number += _number;

            inventoryUpdated();

            return true;
        }

        public void RemoveFromSlot(int _slot, int _number)
        {
            slots[_slot].number -= _number;
            if (slots[_slot].number <= 0)
            {
                slots[_slot].number = 0;
                slots[_slot].item = null;
            }

            inventoryUpdated();
        }

        public bool HasSpaceFor(InventoryItem _item)
        {
            return FindSlot(_item) >= 0;
        }

        public int GetSize()
        {
            return slots.Length;
        }

        public bool HasItem(InventoryItem _item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(slots[i].item, _item))
                {
                    return true;
                }
            }
            return false;
        }

        public InventoryItem GetItemInSlot(int _slot)
        {
            return slots[_slot].item;
        }

        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
        }

        private int FindSlot(InventoryItem _item)
        {
            int i = FindStack(_item);
            if (i < 0)
            {
                i = FindEmptySlot();
            }
            return i;
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindStack(InventoryItem _item)
        {
            if (!_item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(slots[i].item, _item))
                {
                    return i;
                }
            }
            return -1;
        }

        public object CaptureState()
        {
            var slotStrings = new InventorySlotRecord[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                if (slots[i].item != null)
                {
                    slotStrings[i].itemID = slots[i].item.GetItemID();
                    slotStrings[i].number = slots[i].number;
                }
            }
            return slotStrings;
        }

        public void RestoreState(object state)
        {
            var slotStrings = (InventorySlotRecord[])state;
            for (int i = 0; i < inventorySize; i++)
            {
                slots[i].item = InventoryItem.GetFromID(slotStrings[i].itemID);
                slots[i].number = slotStrings[i].number;
            }

            inventoryUpdated();
        }

        public struct InventorySlot
        {
            public InventoryItem item;
            public int number;
        }

        [Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }
    }
}