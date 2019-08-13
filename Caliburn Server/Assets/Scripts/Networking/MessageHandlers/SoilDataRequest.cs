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
        //NetworkClient.Send<SoilDataRequest>(this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        SoilData soilData = Server.Instance.soilDataByNetId[networkId];
        soilId = soilData.soilId;
        soilLayer = soilData.layer;
        position = new Vector2(soilData.posX, soilData.posY);
        NetworkServer.SendToClient<SoilDataRequest>(connection.connectionId, this);
    }
}
