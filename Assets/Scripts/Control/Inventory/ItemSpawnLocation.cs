using RPGProject.Inventories;
using UnityEngine;

namespace RPGProject.Control
{
    /// <summary>
    /// A class that contains which item and how many a pickup will contain.
    /// </summary>
    public class ItemSpawnLocation : MonoBehaviour
    {
        public InventoryItem item = null;
        public int number = 1;
    }
}
