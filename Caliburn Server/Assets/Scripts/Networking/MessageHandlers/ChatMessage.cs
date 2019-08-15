using UnityEngine;
using Mirror;

public class ChatMessage : MessageBase {

    public string playerName;
    public string target;
    public string message;

    public void HandleRequestReceived() {
        Debug.Log("Message received");

        switch(target) {
            case "World":
                NetworkServer.SendToAll<ChatMessage>(this);
            break;

            default:
                //NetworkServer.SendToClient<ChatMessage>(netMsg);
            break;
        }
    }
}
