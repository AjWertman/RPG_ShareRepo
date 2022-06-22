using UnityEngine;
using RPGProject.Saving;

namespace RPGProject.Inventories
{
    /// <summary>
    /// Spawns pickups that should exist on first load in a level. This
    /// automatically spawns the correct prefab for a given inventory item.
    /// </summary>
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        [SerializeField] InventoryItem item = null;
        [SerializeField] int number = 1;

        private void Awake()
        {
            SpawnPickup();
        }

        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        public bool isCollected() 
        { 
            return GetPickup() == null;
        }

        //refactor - change to pool
        private void SpawnPickup()
        {
            var spawnedPickup = item.SpawnPickup(transform.position, number);
            spawnedPickup.transform.SetParent(transform);
        }

        private void DestroyPickup()
        {
            if (GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        public object CaptureState()
        {
            return isCollected();
        }

        public void RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if (shouldBeCollected && !isCollected())
            {
                DestroyPickup();
            }

            if (!shouldBeCollected && isCollected())
            {
                SpawnPickup();
            }
        }
    }
}