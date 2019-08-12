using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInventorySyncRequest : MessageBase {
    public uint networkId;
    public int requestCode;

    public void HandleRequest(NetworkConnection connection) {
        NetworkServer.SendToClient<PlayerInventorySyncRequest>(connection.connectionId, this);
        //NetworkClient.Send<PlayerInventorySyncRequest>(this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        // if server, process request
        ProcessInventory.Instance.LoadRequest(connection, this);
    }
}