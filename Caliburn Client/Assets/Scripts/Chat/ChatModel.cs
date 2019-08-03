using Mirror;

public class ChatModel {
    public string message;

    public void SendMessage() {

        ChatMessage msg = new ChatMessage {
            connectionId = NetworkClient.connection.connectionId,
            target = "World",
            message = this.message,
        };

        msg.HandleMessage();
    }
}
