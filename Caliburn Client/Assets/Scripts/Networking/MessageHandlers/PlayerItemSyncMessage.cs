using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerItemSyncMessage : MessageBase {
    public uint networkId;
    public int itemId;
    public string name;
    public int value;
    public int quantity;
    public bool stackable;
    public string img;
    public int storageType;
    public string primaryUsage;

    public void HandleRequestReceived() {
        if(!InventoryView.Instance.syncing) {return;}
        Item item = new Item {
            id = itemId,
            name = name,
            value = value,
            quantity = quantity,
            stackable = stackable,
            img = img,
            primaryUsage = primaryUsage,
        };
        
        InventoryView.Instance.items.Add(item);
        Debug.Log(name + "x " + quantity);
    }
}
