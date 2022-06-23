using UnityEngine;
using RPGProject.Saving;
using System.Collections.Generic;
using RPGProject.Inventories;

namespace RPGProject.Control
{
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        ItemPickupPool itemPickupPool = null;

        Dictionary<Pickup, Transform> spawnedPickups = new Dictionary<Pickup, Transform>();

        private void Awake()
        {
            itemPickupPool = FindObjectOfType<ItemPickupPool>();
        }

        private void Start()
        {
            InitializePickups();
        }

        private void InitializePickups()
        {
            foreach(ItemSpawnLocation itemSpawnLocation in FindObjectsOfType<ItemSpawnLocation>())
            {
                Transform spawnLocation = itemSpawnLocation.transform;
                InventoryItem inventoryItem = itemSpawnLocation.GetInventoryItem();
                int number = itemSpawnLocation.GetNumber();
                SpawnPickup(spawnLocation, inventoryItem, number);
            }
        }

        public void SpawnPickup(Transform _transform, InventoryItem _inventoryItem, int _number)
        {
            Pickup pickup = itemPickupPool.GetAvailablePickup(_inventoryItem, _number);
            pickup.transform.position = _transform.position;
            pickup.transform.rotation = _transform.rotation;

            pickup.onItemPickup += OnItemPickup;

            spawnedPickups.Add(pickup, pickup.transform);

            pickup.gameObject.SetActive(true);
        }

        private void OnItemPickup(Pickup _pickup, GameObject _null)
        {
            spawnedPickups.Remove(_pickup);
            _pickup.onItemPickup -= OnItemPickup;
        }

        public object CaptureState()
        {
            return spawnedPickups;
        }

        public void RestoreState(object state)
        {
            spawnedPickups = (Dictionary<Pickup,Transform>)state;

            foreach(Pickup pickup in spawnedPickups.Keys)
            {
                Transform spawnTransform = spawnedPickups[pickup];
                pickup.transform.position = spawnTransform.position;
                pickup.transform.rotation = spawnTransform.rotation;

                pickup.gameObject.SetActive(true);
            }
        }
    }
}