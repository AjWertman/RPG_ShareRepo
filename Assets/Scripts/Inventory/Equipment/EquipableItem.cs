using UnityEngine;

namespace RPGProject.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        [SerializeField] EquipmentSlot equipmentSlot = EquipmentSlot.None;

        public EquipmentSlot GetEquipmentSlot()
        {
            return equipmentSlot;
        }
    }
}