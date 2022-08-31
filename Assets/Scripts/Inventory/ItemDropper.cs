using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPGProject.Saving;
using RPGProject.Inventories;
using System;

namespace RPGProject.Control
{
    /// <summary>
    /// Handles the dropping of items.
    /// </summary>
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        public Transform itemDropTransform = null;

        PickupSpawner pickupSpawner = null;

        List<Pickup> droppedItems = new List<Pickup>();
        List<DropRecord> otherSceneDroppedItems = new List<DropRecord>();

        private void Awake()
        {
            pickupSpawner = FindObjectOfType<PickupSpawner>();
        }

        public void DropItem(InventoryItem _inventoryItem, int _number)
        {
            SpawnPickup(itemDropTransform, _inventoryItem, _number);
        }

        public void DropItem(InventoryItem _inventoryItem)
        {
            SpawnPickup(itemDropTransform, _inventoryItem, 1);
        }

        public void SpawnPickup(Transform _spawnTransform, InventoryItem _inventoryItem, int _number)
        {
            Pickup pickup = pickupSpawner.SpawnPickup(_spawnTransform, _inventoryItem, _number);

            droppedItems.Add(pickup);
        }

        public void SpawnPickup(Vector3 _spawnPosition, InventoryItem _inventoryItem, int _number)
        {
            Pickup pickup = pickupSpawner.SpawnPickup(_spawnPosition, _inventoryItem, _number);

            droppedItems.Add(pickup);
        }

        //Refactor below
        [Serializable]
        private struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
            public int sceneBuildIndex;
        }

        public object CaptureState()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new List<DropRecord>();
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            foreach (Pickup pickup in droppedItems)
            {
                var droppedItem = new DropRecord();
                droppedItem.itemID = pickup.item.itemID;
                droppedItem.position = new SerializableVector3(pickup.transform.position);
                droppedItem.number = pickup.number;
                droppedItem.sceneBuildIndex = buildIndex;
                droppedItemsList.Add(droppedItem);
            }
            droppedItemsList.AddRange(otherSceneDroppedItems);
            return droppedItemsList;
        }

        public void RestoreState(object state)
        {
            var droppedItemsList = (List<DropRecord>)state;
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            otherSceneDroppedItems.Clear();
            foreach (var item in droppedItemsList)
            {
                if (item.sceneBuildIndex != buildIndex)
                {
                    otherSceneDroppedItems.Add(item);
                    continue;
                }
                var pickupItem = InventoryItem.GetFromID(item.itemID);
                Vector3 position = item.position.ToVector3();
                int number = item.number;
                SpawnPickup(position, pickupItem, number);
            }
        }

        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
}