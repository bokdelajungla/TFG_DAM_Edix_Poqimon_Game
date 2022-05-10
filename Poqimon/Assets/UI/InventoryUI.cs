using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    int selectedItem = 0;

    List<ItemSlotUI> slotUIList;
    Inventory inventory;

    private void Awake() {
        inventory = Inventory.getInventory();
    }

    private void Start() {
        updateItemList();
    }

    void updateItemList() {
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots) {
           var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.setData(itemSlot);
            slotUIList.Add(slotUIObj);
        }

        updateItemSelection();
    }
    public void HandleUpdate(Action onBack) {

        int prevSelection = selectedItem;
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count -1);

        if (prevSelection != selectedItem) {
            updateItemSelection();  
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            onBack.Invoke();
        }
    }

    public void updateItemSelection() {
        for (int i = 0; i < slotUIList.Count; i++) {
            if (i == selectedItem)
                slotUIList[i].NameText.color = Color.blue;
            else 
                slotUIList[i].NameText.color = Color.black;
        }

        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;
    }
}
