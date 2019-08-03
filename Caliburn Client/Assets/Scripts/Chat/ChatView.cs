using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour {
    public InputField chatMessageInput;

    public void ProcessChatMessage() {
        string msg = chatMessageInput.text;

        if(!IsNullOrWhiteSpace(msg)) {
            ChatModel newChatMessage = new ChatModel {
                message = msg
            };

            newChatMessage.SendMessage();
        }

        chatMessageInput.text = "";
    }

    bool IsNullOrWhiteSpace(string message) {
        if(message == null) {return true;}

        for(int i = 0; i < message.Length; i++) {
            if( message[i] != ' ' &&
                message[i] != '\n' &&
                message[i] != '\r' &&
                message[i] != '\t' ) {
                return false;
            }
        }

        return true;
    }
}
