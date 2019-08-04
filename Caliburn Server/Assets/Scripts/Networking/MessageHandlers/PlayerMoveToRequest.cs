using UnityEngine;
using Mirror;

public class PlayerMoveToRequest : MessageBase {
    public Vector2 position;

    public void HandleRequest() {
        //NetworkClient.Send<PlayerMoveToRequest>(this);
    }

    public void HandleRequestReceived() {
        //NetworkServer.SendToAll<PlayerMoveToRequest>(this);
    }
}