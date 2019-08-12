using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventorySyncRequest : MessageBase {
    public uint networkId;
    public int requestCode;

    public void HandleRequest() {
        Debug.Log("Requesting inventory data...");
        NetworkClient.Send<PlayerInventorySyncRequest>(this);
    }

    public void HandleRequestReceived() {
        switch(requestCode) {
            case 0:
                // create a new inventory, disable inventory actions.
                InventoryView.Instance.BeginSync();
            break;

            case 1:
                //re-enable inventory actions.
                InventoryView.Instance.EndSync();
            break;
        }
    }
}