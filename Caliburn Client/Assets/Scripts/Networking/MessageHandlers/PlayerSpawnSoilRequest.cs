using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawnSoilRequest : MessageBase {
    public string ownerId;
    public uint networkId;
    public int soilId;
    public int soilLayer;
    public Vector2 position;
    public bool complete = false;

    public void HandleRequest() {
        Debug.Log(position);
        NetworkClient.Send<PlayerSpawnSoilRequest>(this);
    }
}
