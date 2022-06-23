using UnityEngine;

namespace RPGProject.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }
    }
}