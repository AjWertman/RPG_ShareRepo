using RPGProject.Inventories;
using System;
using UnityEngine;

namespace RPGProject.Control
{
    /// <summary>
    /// The interactable object in the overworld that stores the 
    /// inventory data of an inventory item.
    /// </summary>
    public class Pickup : MonoBehaviour, IRaycastable
    {
        public InventoryItem item;
        public int number = 1;

        GameObject itemMesh = null;

        public event Action<Pickup, GameObject> onItemPickup;

        public void Setup(InventoryItem _item, GameObject _itemMesh, int _number)
        {
            item = _item;
            itemMesh = _itemMesh;
            if (!item.isStackable)
            {
                _number = 1;
            }

            itemMesh.transform.parent = transform;
            itemMesh.transform.localPosition = Vector3.zero;
            itemMesh.transform.localEulerAngles = Vector3.zero;
            itemMesh.gameObject.SetActive(true);

            number = _number;
        }

        public void ResetPickup()
        {
            item = null;
            number = 0;
        }

        public bool HandleRaycast(PlayerController _playerController)
        {
            return true;
        }

        public string WhatToActivate()
        {
            return "Pickup " + item.displayName + "?";
        }

        public void WhatToDoOnClick(PlayerController _playerController)
        {
            Inventory inventory = _playerController.GetInventory();
            if (!inventory.HasSpaceFor(item)) return;
            inventory.AddToFirstEmptySlot(item, number);
            onItemPickup(this, itemMesh);
        }
    }
}