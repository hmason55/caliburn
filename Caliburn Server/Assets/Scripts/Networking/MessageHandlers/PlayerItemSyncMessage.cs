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

    public void HandleRequest(NetworkConnection connection) {
        NetworkServer.SendToClient<PlayerItemSyncMessage>(connection.connectionId, this);
    }

    public void HandleRequestReceived() {

    }
}
