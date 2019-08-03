using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChatMessage : MessageBase {

    public int connectionId;
    public string target;
    public string message;

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
