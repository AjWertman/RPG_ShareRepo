using UnityEngine;
using zGameDevTV.Core.UI.Dragging;
using RPGProject.Inventories;

namespace RPGProject.UI
{
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;

        Equipment playerEquipment;

        private void Awake() 
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponentInChildren<Equipment>();
            playerEquipment.equipmentUpdated += RedrawUI;
        }

        private void Start() 
        {
            RedrawUI();
        }

        public void AddItems(InventoryItem _item, int _number)
        {
            playerEquipment.AddItem(equipLocation, (EquipableItem)_item);
        }

        public void RemoveItems(int _number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        public int MaxAcceptable(InventoryItem _item)
        {
            EquipableItem equipableItem = _item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.GetAllowedEquipLocation() != equipLocation) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
        }
    }
}