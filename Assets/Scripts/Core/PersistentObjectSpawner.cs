using UnityEngine;

namespace RPGProject.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectsPrefab = null;

        static bool hasSpawned = false;

        private void Awake()
        {
            if (!hasSpawned)
            {
                hasSpawned = true;
                SpawnPersistentObjects();
            }
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistentObjectsInstance = Instantiate(persistentObjectsPrefab);
        }
    }
}