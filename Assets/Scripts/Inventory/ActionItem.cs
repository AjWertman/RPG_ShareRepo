using UnityEngine;

namespace RPGProject.Inventories
{
    /// <summary>
    /// Item that can be used.
    /// </summary>
    [CreateAssetMenu(menuName = ("Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        public bool isConsumable = false;

        public virtual void Use(GameObject _user)
        {
            Debug.Log("Using action: " + this);
        }
    }
}