using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerDataSyncMessage : IMessageBase  {
    public uint networkId;
    public int connectionId;
    public string username;
    public string ipAddress;
    public Vector2 position;
    public Vector2 destination;

    public void HandleRequest() {
        NetworkServer.SendToAll<PlayerDataSyncMessage>(this);
    }

    public void HandleRequest(NetworkConnection connection) {
        NetworkServer.SendToClient<PlayerDataSyncMessage>(connection.connectionId, this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        HandleRequest(connection);
    }

    public void Deserialize(NetworkReader reader) {
        networkId = reader.ReadPackedUInt32();
        connectionId = reader.ReadPackedInt32();
        username = reader.ReadString();
        ipAddress = reader.ReadString();
        position = reader.ReadVector2();
        destination = reader.ReadVector2();
    }

    public void Serialize(NetworkWriter writer) {
        writer.WritePackedUInt32(networkId);
        writer.WritePackedInt32(connectionId);
        writer.WriteString(username);
        writer.WriteString(ipAddress);
        writer.WriteVector2(position);
        writer.WriteVector2(destination);
    }
}
