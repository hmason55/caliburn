using Mirror;

public class ChatModel {
    public string message;

    public void SendMessage() {

        ChatMessage msg = new ChatMessage {
            playerName = PlayerInfo.Instance.PlayerName,
            target = "World",
            message = this.message,
        };

        msg.HandleMessage();
    }
}
