using UnityEngine;
using Mirror;

public class ChatMessage : MessageBase {

    public string playerName;
    public string target;
    public string message;

    public override void Deserialize(NetworkReader reader) {
        playerName = reader.ReadString();
        target = reader.ReadString();
        message = reader.ReadString();
    }

    public override void Serialize(NetworkWriter writer) {
        writer.WriteString(playerName);
        writer.WriteString(target);
        writer.WriteString(message);
    }

    public void HandleMessage() {

        // Logic for illegal characters.
        if(message.Contains("shit")) {
            Debug.Log("You can't say 'shit' over the network.");
        } else {
            Debug.Log("Message is clear, sending...");
            NetworkClient.Send<ChatMessage>(this);
        }
    }
}
