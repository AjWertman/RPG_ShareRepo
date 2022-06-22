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
            AddToFirstEmptySlot(potion, 6);
        }

        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if (slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, number); ;
            }

            var i = FindStack(item);
            if (i >= 0)
            {
                slot = i;
            }

            slots[slot].item = item;
            slots[slot].number += number;

            inventoryUpdated();

            return true;
        }

        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            slots[i].item = item;
            slots[i].number += number;

            inventoryUpdated();

            return true;
        }

        public void RemoveFromSlot(int slot, int number)
        {
            slots[slot].number -= number;
            if (slots[slot].number <= 0)
            {
                slots[slot].number = 0;
                slots[slot].item = null;
            }

            inventoryUpdated();
        }

        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        public int GetSize()
        {
            return slots.Length;
        }

        public bool HasItem(InventoryItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(slots[i].item, item))
                {
                    return true;
                }
            }
            return false;
        }

        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }

        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
        }

        private int FindSlot(InventoryItem item)
        {
            int i = FindStack(item);
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

        private int FindStack(InventoryItem item)
        {
            if (!item.IsStackable())
            {
                return -1;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                if (ReferenceEquals(slots[i].item, item))
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