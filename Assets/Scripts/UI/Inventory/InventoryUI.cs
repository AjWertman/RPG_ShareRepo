using System.Collections.Generic;
using UnityEngine;
using RPGProject.Inventories;

namespace RPGProject.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] InventorySlotUI inventoryItemPrefab = null;
        [SerializeField] int amountOfSlotUIsToCreate = 50;

        Inventory inventory;

        List<InventorySlotUI> inventorySlotUIs = new List<InventorySlotUI>();

        private void Awake() 
        {
            //Refactor
            return;
            inventory = FindObjectOfType<Inventory>();
            inventory.inventoryUpdated += Redraw;
            CreateInventorySlotUIs();
        }

        private void Start()
        {
            return;
            Redraw();
        }

        private void Redraw()
        {
            DisableAllSlots();

            for (int i = 0; i < inventory.GetSize(); i++)
            {
                InventorySlotUI slotUI = GetAvailableInventorySlotUI();
                slotUI.Setup(i);
                slotUI.gameObject.SetActive(true);
            }
        }

        private void CreateInventorySlotUIs()
        {
            for (int i = 0; i < amountOfSlotUIsToCreate; i++)
            {
                InventorySlotUI slotUIInstance = Instantiate(inventoryItemPrefab, transform);
                slotUIInstance.InitializeSlotUI(inventory);
                inventorySlotUIs.Add(slotUIInstance);
            }

            DisableAllSlots();
        }

        private void DisableAllSlots()
        {
            foreach (InventorySlotUI slotUI in inventorySlotUIs)
            {
                slotUI.ResetSlot();
                slotUI.gameObject.SetActive(false);
            }
        }

        private InventorySlotUI GetAvailableInventorySlotUI()
        {
            InventorySlotUI slotToGet = null;

            foreach(InventorySlotUI slotUI in inventorySlotUIs)
            {
                if (!slotUI.gameObject.activeSelf)
                {
                    slotToGet = slotUI;
                    break;
                }
            }

            return slotToGet;
        }
    }
}