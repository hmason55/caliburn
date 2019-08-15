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
        foreach(KeyValuePair<uint, PlayerData>  playerData in Server.Instance.playerDataByNetId) {
            
            PlayerView playerView = NetworkIdentity.spawned[playerData.Key].GetComponent<PlayerView>();

            PlayerDataSyncMessage playerDataMessage = new PlayerDataSyncMessage {
                networkId = playerData.Key,
                connectionId = playerData.Value.connectionId,
                username = playerData.Value.username,
                ipAddress = playerData.Value.ipAddress,
                position = (Vector2)playerView.transform.position,
                destination = playerData.Value.destination,
            };

            // Send to target client.
            playerDataMessage.HandleRequest(connection);
        }
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
