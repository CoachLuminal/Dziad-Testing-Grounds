using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Stash")]
    [SerializeField] private int stashSize = 100;
    [SerializeField] public int maxWeight = 100;
    [SerializeField] public int currentWeight = 0;
    [SerializeField] private int currentStashSize;

    [Header("Inventory Items")]
    [SerializeField] private GameObject inventoryParent;
    [SerializeField] private List<Interactable> inventoryItems = new List<Interactable>();

    [Header("Inventory Bool")]
    [SerializeField] public bool isInventoryOpen = false;

    [Header("Inventory UI")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private TextMesh stashText;

    void Start()
    {
        
    }

    void Update()
    {
        OpenInventory();
    }
    
    void OpenInventory()
    {
        Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;

        if (Input.GetKeyDown(KeyCode.I))
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryUI.SetActive(isInventoryOpen);
            Debug.Log(isInventoryOpen ? "Inventory Opened" : "Inventory Closed");
            
            if (isInventoryOpen)
            {
                DisplayInventoryStash();
                
            }
        }
    }

    void DisplayInventoryStash()
    {
        string stashInfo = "Inventory:\n";
        foreach (var item in inventoryItems)
        {
            stashInfo += item.itemID + " - " + item.itemName + " (Weight: " + item.itemWeight + ")\n";
        }
        stashText.text = stashInfo;

        
    }

    public void InventoryAddItem(Interactable item)
    {
        if (currentWeight + item.itemWeight <= maxWeight)
        {
            currentStashSize++;
            GameObject itemObj = new GameObject("InventoryItem_" + item.itemName);
            itemObj.transform.SetParent(inventoryParent.transform);
            Interactable newItem = itemObj.AddComponent<Interactable>();
            newItem.itemID = currentStashSize;
            newItem.itemName = item.itemName;
            newItem.itemWeight = item.itemWeight;
            newItem.itemDescription = item.itemDescription;
            newItem.itemIcon = item.itemIcon;   

            inventoryItems.Add(newItem);
            currentWeight += item.itemWeight;
            Debug.Log("Added " + item.itemName + " to inventory.");
        } else
        {
                Debug.Log("Cannot pick up " + gameObject.name + ". Inventory is full or overweight.");
        }
    }
}
