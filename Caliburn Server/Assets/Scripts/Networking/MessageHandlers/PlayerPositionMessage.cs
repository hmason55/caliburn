using UnityEngine;
using Mirror;

public class PlayerPositionMessage : MessageBase {
    public uint networkId;
    public Vector2 position;

    public void HandleRequest() {
        // Server stuff
        NetworkServer.SendToAll<PlayerPositionMessage>(this);
    }

    public void HandleRequestReceived(NetworkConnection connection) {
        // Client stuff
    }
}