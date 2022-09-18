using RPGProject.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
    /// <summary>
    /// Pools the pickups and their meshes for use in the overworld.
    /// </summary>
    public class ItemPickupPool : MonoBehaviour
    {
        [SerializeField] Transform pickupParent = null;
        [SerializeField] Transform meshParent = null;

        [SerializeField] Pickup pickupPrefab = null;
        [SerializeField] int amountOfPickPrefabsToSpawn = 10;

        [SerializeField] ItemMeshPrefab[] itemMeshPrefabs = null;

        List<Pickup> pickupPool = new List<Pickup>();
        Dictionary<ItemMeshKey, List<GameObject>> itemMeshPool= new Dictionary<ItemMeshKey, List<GameObject>>();

        private void Awake()
        {
            CreatePickupsPool();
            CreateItemMeshPool();
        }

        private void SetupPickup(Pickup pickup, InventoryItem _inventoryItem, int _number)
        {
            ItemMeshKey itemMeshKey = _inventoryItem.itemMeshKey;
            GameObject availableMesh = GetAvailableItemMesh(itemMeshKey);
            pickup.Setup(_inventoryItem, availableMesh, _number);
        }

        public GameObject GetAvailableItemMesh(ItemMeshKey _itemMeshKey)
        {
            GameObject availableItemMesh = null;
            List<GameObject> itemMeshes = itemMeshPool[_itemMeshKey];
            foreach (GameObject itemMesh in itemMeshes)
            {
                if (itemMesh.gameObject.activeSelf) continue;

                availableItemMesh = itemMesh;
                break;
            }

            return availableItemMesh;
        }

        private void CreatePickupsPool()
        {
            for (int i = 0; i < amountOfPickPrefabsToSpawn; i++)
            {
                Pickup pickup = Instantiate(pickupPrefab, pickupParent);
                pickup.onItemPickup += ReturnPickup;
                
                pickup.ResetPickup();

                pickupPool.Add(pickup);
                pickup.gameObject.SetActive(false);
            }
        }

        private void ReturnPickup(Pickup _pickup, GameObject _itemMesh)
        {
            _pickup.ResetPickup();
            _pickup.transform.localPosition = Vector3.zero;
            _pickup.transform.localEulerAngles = Vector3.zero;
            _pickup.gameObject.SetActive(false);

            _itemMesh.transform.parent = meshParent;
            _itemMesh.transform.localPosition = Vector3.zero;
            _itemMesh.transform.localEulerAngles = Vector3.zero;
            _itemMesh.gameObject.SetActive(false);
        }

        private void CreateItemMeshPool()
        {
            foreach(ItemMeshPrefab itemMeshPrefab in itemMeshPrefabs)
            {
                List<GameObject> itemMeshes = new List<GameObject>();

                for (int i = 0; i < itemMeshPrefab.amountToSpawn; i++)
                {
                    GameObject itemMesh = Instantiate(itemMeshPrefab.itemMeshPrefab, meshParent);
                    itemMeshes.Add(itemMesh);
                    itemMesh.gameObject.SetActive(false);
                }

                itemMeshPool.Add(itemMeshPrefab.itemMeshKey, itemMeshes);
            }
        }

        public Pickup GetAvailablePickup(InventoryItem _inventoryItem, int _number)
        {
            Pickup availablePickup = null;

            foreach (Pickup pickup in pickupPool)
            {
                if (pickup.gameObject.activeSelf) continue;

                availablePickup = pickup;
                break;
            }

            SetupPickup(availablePickup, _inventoryItem, _number);
            return availablePickup;
        }

        [Serializable]
        private class ItemMeshPrefab
        {
            public ItemMeshKey itemMeshKey = ItemMeshKey.None;
            public GameObject itemMeshPrefab = null;
            public int amountToSpawn = 4;
        }
    }

}
