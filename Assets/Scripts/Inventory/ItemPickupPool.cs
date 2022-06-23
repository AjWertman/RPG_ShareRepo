using RPGProject.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Control
{
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

        public Pickup GetAvailablePickup(InventoryItem _inventoryItem, int _number)
        {
            Pickup availablePickup = null;

            foreach(Pickup pickup in pickupPool)
            {
                if (pickup.gameObject.activeSelf) continue;

                availablePickup = pickup;
                break;
            }

            SetupPickup(availablePickup, _inventoryItem, _number);
            return availablePickup;
        }

        private void SetupPickup(Pickup pickup, InventoryItem _inventoryItem, int _number)
        {
            ItemMeshKey itemMeshKey = _inventoryItem.GetItemMeshKey();
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
                Pickup pickup = Instantiate(pickupPrefab);
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

                for (int i = 0; i < itemMeshPrefab.GetAmountToSpawn(); i++)
                {
                    GameObject itemMesh = Instantiate(itemMeshPrefab.GetItemMeshPrefab(), meshParent);
                    itemMeshes.Add(itemMesh);
                    itemMesh.gameObject.SetActive(false);
                }

                itemMeshPool.Add(itemMeshPrefab.GetItemMeshKey(), itemMeshes);
            }
        }

        [Serializable]
        private class ItemMeshPrefab
        {
            [SerializeField] ItemMeshKey itemMeshKey = ItemMeshKey.None;
            [SerializeField] GameObject itemMeshPrefab = null;
            [SerializeField] int amountToSpawn = 4;
            
            public ItemMeshKey GetItemMeshKey()
            {
                return itemMeshKey;
            }

            public GameObject GetItemMeshPrefab()
            {
                return itemMeshPrefab;
            }

            public int GetAmountToSpawn()
            {
                return amountToSpawn;
            }
        }
    }

}
