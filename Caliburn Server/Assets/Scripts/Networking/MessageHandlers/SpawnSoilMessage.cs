using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnSoilMessage : MessageBase {
    public uint networkId;
    public int soilId;
    public int soilLayer;
    public Vector2 position;

    public void HandleRequest() {
        NetworkServer.SendToAll<SpawnSoilMessage>(this);
    }
}
