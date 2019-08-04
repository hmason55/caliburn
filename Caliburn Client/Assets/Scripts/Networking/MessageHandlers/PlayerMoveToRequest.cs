using UnityEngine;
using Mirror;

public class PlayerMoveToRequest : MessageBase {
    public Vector2 position;

    public void HandleRequest() {
        Debug.Log("Movement request sent.");
        NetworkClient.Send<PlayerMoveToRequest>(this);
    }
}