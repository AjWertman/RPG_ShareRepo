using UnityEngine;
using RPGProject.Inventories;

namespace RPGProject.UI
{
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipmentSlot equipmentSlot = EquipmentSlot.None;

        Equipment playerEquipment;

        //Refactor - Needs to handle all characters equipmunk
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
            playerEquipment.AddItem(equipmentSlot, (EquipableItem)_item);
        }

        public void RemoveItems(int _number)
        {
            playerEquipment.RemoveItem(equipmentSlot);
        }

        public int MaxAcceptable(InventoryItem _item)
        {
            EquipableItem equipableItem = _item as EquipableItem;
            if (equipableItem == null) return 0;
            if (equipableItem.equipmentSlot != equipmentSlot) return 0;
            if (GetItem() != null) return 0;

            return 1;
        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipmentSlot);
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
            icon.SetItem(playerEquipment.GetItemInSlot(equipmentSlot));
        }
    }
}