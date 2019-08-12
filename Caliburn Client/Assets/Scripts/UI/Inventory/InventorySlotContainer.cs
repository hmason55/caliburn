using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotContainer : MonoBehaviour {
    public List<ItemSlot> slots;

    void Awake() {
        slots = new List<ItemSlot>(GetComponentsInChildren<ItemSlot>());
    }

    public void LoadItems(List<Item> items) {
        for(int i = 0; i < slots.Count; i++) {
            if(i < items.Count) {
                Debug.Log(i);
                slots[i].LoadItem(i, items[i]);
            } else {
                // empty
                slots[i].Clear();
            }
        }
    }
}
