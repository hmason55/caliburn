using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SoilDataRequest : MessageBase {
    public string ownerId;
    public uint networkId;
    public int soilId;
    public int soilLayer;
    public Vector2 position;

    public void HandleRequest() {
        NetworkClient.Send<SoilDataRequest>(this);
    }

    public void HandleRequestReceived() {
        NetworkIdentity identity = NetworkIdentity.spawned[networkId];

        SoilView soilView = identity.GetComponent<SoilView>();
        if(soilView == null) {return;}
        soilView.UpdateSoilData(this);
    }
}
