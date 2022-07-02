using RPGProject.Inventories;
using UnityEngine;

namespace RPGProject.Control
{
    public class ItemSpawnLocation : MonoBehaviour
    {
        [SerializeField] InventoryItem item = null;
        [SerializeField] int number = 1;

        public InventoryItem GetInventoryItem()
        {
            return item;
        }

        public int GetNumber()
        {
            return number;
        }
    }
}
