using UnityEngine;
using Mirror;

public class PlayerMoveToRequest : MessageBase {
    public uint networkId;
    public Vector2 destination;

    public void HandleRequest() {
        // Client stuff
        NetworkClient.Send<PlayerMoveToRequest>(this);
    }

    public void HandleRequestReceived(NetworkIdentity identity) {
        PlayerView playerView = identity.GetComponent<PlayerView>();
        playerView.localDestination = destination;
    }
}