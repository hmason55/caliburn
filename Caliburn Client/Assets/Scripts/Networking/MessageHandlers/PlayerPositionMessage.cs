using UnityEngine;
using Mirror;

public class PlayerPositionMessage : MessageBase {
    public uint networkId;
    public Vector2 position;

    public void HandleRequest() {
        // Server stuff
    }

    public void HandleRequestReceived(NetworkIdentity identity) {
        // Client stuff
        if(identity == null) {return;}

        PlayerView playerView = identity.GetComponent<PlayerView>();
        if(playerView == null) {return;}

        playerView.actualPosition = position;//Vector2.Lerp((Vector2)playerView.transform.position, position, Time.deltaTime * 5f);
    }
}