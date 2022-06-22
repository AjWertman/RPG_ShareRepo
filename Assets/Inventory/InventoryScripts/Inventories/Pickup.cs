using UnityEngine;

namespace RPGProject.Inventories
{
    public class Pickup : MonoBehaviour
    {
        Inventory inventory;

        InventoryItem item;
        int number = 1;

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        public void Setup(InventoryItem _item, int _number)
        {
            item = _item;
            if (!item.IsStackable())
            {
                _number = 1;
            }
            number = _number;
        }

        public void PickupItem()
        {
            //Refactor - return to pool
            bool foundSlot = inventory.AddToFirstEmptySlot(item, number);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetNumber()
        {
            return number;
        }

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item);
        }
    }
}