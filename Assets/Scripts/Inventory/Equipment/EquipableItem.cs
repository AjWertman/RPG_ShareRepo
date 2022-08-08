using UnityEngine;

namespace RPGProject.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        public EquipmentSlot equipmentSlot = EquipmentSlot.None;
    }
}