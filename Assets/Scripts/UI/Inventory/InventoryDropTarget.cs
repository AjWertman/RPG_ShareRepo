using UnityEngine;
using RPGProject.Inventories;

namespace RPGProject.UI
{
    /// <summary>
    /// The area outside the canvas that will trigger an item drop.
    /// </summary>
    public class InventoryDropTarget : MonoBehaviour, IDragDestination<InventoryItem>
    {
        public void AddItems(InventoryItem item, int number)
        {
            //Refactor
            //var player = GameObject.FindGameObjectWithTag("Player");
            //player.GetComponent<ItemDropper>().DropItem(item, number);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return int.MaxValue;
        }
    }
}