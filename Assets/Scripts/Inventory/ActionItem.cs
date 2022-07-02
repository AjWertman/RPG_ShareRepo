using UnityEngine;

namespace RPGProject.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        [SerializeField] bool isConsumable = false;

        public virtual void Use(GameObject _user)
        {
            Debug.Log("Using action: " + this);
        }

        public bool IsConsumable()
        {
            return isConsumable;
        }
    }
}