using Mirror;
using UnityEngine;

public class PlayerDataSyncMessage : IMessageBase  {
    public uint networkId;
    public int connectionId;
    public string username;
    public string ipAddress;
    Vector2 position;
    Vector2 destination;

    public void HandleRequest(NetworkConnection connection) {
        NetworkClient.Send<PlayerDataSyncMessage>(this);
    }

    public void HandleRequestReceived() {
        PlayerData playerData = new PlayerData {
            networkId = networkId,
            connectionId = connectionId,
            username = username,
            ipAddress = ipAddress,
            position = position,
            destination = destination,
        };

        /*
        Debug.Log("networkId: " + networkId);
        Debug.Log("connectionId: " + connectionId);
        Debug.Log("username: " + username);
        Debug.Log("ipAddress: " + ipAddress);
        Debug.Log("position: " + position);
        Debug.Log("destination: " + destination);
        */

        NetworkIdentity identity = NetworkIdentity.spawned[networkId];
        if(identity == null) {return;}

        PlayerView playerView = identity.GetComponent<PlayerView>();
        if(playerView == null) {return;}

        playerView.playerData = playerData;
        playerView.HandlePlayerData();
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
