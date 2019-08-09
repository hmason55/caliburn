using Mirror;

public class GrowableDataRequest : MessageBase {
    public string ownerId;
    public uint networkId;
    public int creationDate;
    public int completionDate;

    public void HandleRequest() {
        //NetworkClient.Send(this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        GrowableData growableData = Server.Instance.growableDataByNetId[networkId];
        ownerId = growableData.ownerId;
        creationDate = growableData.creationDate;
        completionDate = growableData.completionDate;
        NetworkServer.SendToClient<GrowableDataRequest>(connection.connectionId, this);
    }
}