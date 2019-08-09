using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNameplateMessage : MessageBase {
    public uint networkId;
    public string username;
    public string title;

    public void HandleRequest() {
        // Server
    }

    public void HandleRequestReceived(NetworkIdentity identity) {
        PlayerView playerView = identity.GetComponent<PlayerView>();
        playerView.nameplate.text = title + username;
    }
}
