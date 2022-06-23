using System;
using UnityEngine;

namespace RPGProject.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        [SerializeField] bool consumable = false;

        public virtual void Use(GameObject _user)
        {
            Debug.Log("Using action: " + this);
        }

        public bool isConsumable()
        {
            return consumable;
        }
    }
}