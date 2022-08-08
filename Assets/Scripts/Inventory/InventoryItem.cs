using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Inventories
{
    public enum ItemMeshKey { None, Bag, Weapon, Armor, Potion, Food }

    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        public string itemID = null;
        public string displayName = null;
        [TextArea] public string description = null;
        public Sprite icon = null;
        public ItemMeshKey itemMeshKey = ItemMeshKey.None;
        public bool isStackable = false;

        static Dictionary<string, InventoryItem> itemLookupCache;

        //refactor - change to a database of inventory items/Addressables/Assets Bundles?
        public static InventoryItem GetFromID(string _itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, InventoryItem>();
                InventoryItem[] itemList = Resources.LoadAll<InventoryItem>("");
                foreach (InventoryItem item in itemList)
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
