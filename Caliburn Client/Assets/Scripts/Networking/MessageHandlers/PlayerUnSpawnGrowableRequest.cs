using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerUnSpawnGrowableRequest : MessageBase {
    public string ownerId;
    public uint networkId;

    public void HandleRequest() {
        Debug.Log("sending request for destroy");
        Debug.Log(networkId);
        NetworkClient.Send<PlayerUnSpawnGrowableRequest>(this);
    }

    public void HandleRequestReceived() {

    }
}
