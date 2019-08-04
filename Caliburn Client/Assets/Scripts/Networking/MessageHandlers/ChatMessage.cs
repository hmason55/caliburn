using UnityEngine;
using Mirror;

public class ChatMessage : MessageBase {

    public string playerName;
    public string target;
    public string message;

    public void HandleMessage() {

        // Logic for illegal characters.
        if(message.Contains("shit")) {
            Debug.Log("You can't say 'shit' over the network.");
        } else {
            NetworkClient.Send<ChatMessage>(this);
        }
    }

    public void HandleMessageReceived() {
        switch(target) {
            case "World":
                ChatController.Instance.CreateNewMessage(playerName, message);
            break;
        }
    }
}
