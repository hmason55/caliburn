using Mirror;
using UnityEngine;

public class GrowableDataRequest : MessageBase {

    public uint networkId;
    public int uniqueId;
    public string ownerId;
    public string growableId;
    public Vector2 position;
    public int creationDate;
    public int completionDate;
    public int waterDate;

    public void HandleRequest() {
        NetworkServer.SendToAll<GrowableDataRequest>(this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        GrowableData growableData = Server.Instance.growableDataByNetId[networkId];
        uniqueId = growableData.uniqueId;
        ownerId = growableData.ownerId;
        growableId = growableData.growableId;
        position = new Vector2(growableData.posX, growableData.posY);
        creationDate = growableData.creationDate;
        completionDate = growableData.completionDate;
        waterDate = growableData.waterDate;
        NetworkServer.SendToClient<GrowableDataRequest>(connection.connectionId, this);
    }
}