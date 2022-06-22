﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Inventories
{
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] string itemID = null;
        [SerializeField] string displayName = null;
        [SerializeField][TextArea] string description = null;
        [SerializeField] Sprite icon = null;

        [SerializeField] Pickup pickup = null;
        [SerializeField] bool stackable = false;

        static Dictionary<string, InventoryItem> itemLookupCache;

        //refactor - change to a database of inventory items
        public static InventoryItem GetFromID(string _itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, InventoryItem>();
                var itemList = Resources.LoadAll<InventoryItem>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (_itemID == null || !itemLookupCache.ContainsKey(_itemID)) return null;
            return itemLookupCache[_itemID];
        }
        
        public Pickup SpawnPickup(Vector3 _position, int _number)
        {
            //Refactor - add Pickup pool
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = _position;
            pickup.Setup(this, _number);
            return pickup;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }
        
        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(itemID))
            {
                itemID = Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {

        }
    }
}
