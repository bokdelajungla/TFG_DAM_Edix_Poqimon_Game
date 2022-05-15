using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;

    public List<ItemSlot> Slots => slots;

    public static Inventory getInventory() {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public void addItem(ItemBase item, int count =1) {
        Debug.Log(item);
        Debug.Log(slots.Count);
       slots.Add(new ItemSlot()
       {
        Item = item,
        Count = count
       });
       Debug.Log(slots.Count);
    }



}

[Serializable]
public class ItemSlot {
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item {
        get => item;
        set => item = value;
    }

    public int Count {
        get => count;
        set => count = value;
    }


}
